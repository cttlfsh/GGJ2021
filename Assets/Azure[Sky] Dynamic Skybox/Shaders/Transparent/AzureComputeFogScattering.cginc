// Constants
#define PI 3.1415926535
#define Pi316 0.0596831
#define Pi14 0.07957747
#define MieG float3(0.4375f, 1.5625f, 1.5f)
static const float3 betaR = float3(5.8e-3, 1.35e-2, 3.31e-2);
static const float mieG = 0.8;
static const int RES_R = 32;
static const int RES_MU = 128;
static const int RES_MU_S = 32;
static const int RES_NU = 8;

uniform int       _Azure_StylizedTransmittanceMode, _Azure_SkyModel;
uniform float3    _Azure_SunDirection, _Azure_MoonDirection;
uniform float3    _Azure_Br, _Azure_Bm;
uniform float     _Azure_ScatteringIntensity, _Azure_SkyLuminance, _Azure_Exposure;
uniform float3    _Azure_RayleighColor, _Azure_MieColor, _Azure_TransmittanceColor;
uniform float     _Azure_GlobalFogSmooth, _Azure_GlobalFogDistance, _Azure_GlobalFogDensity;
uniform float     _Azure_HeightFogSmooth, _Azure_HeightFogDistance, _Azure_HeightFogDensity, _Azure_HeightFogStart, _Azure_HeightFogEnd;

uniform float4x4  _Azure_SunMatrix, _Azure_MoonMatrix, _Azure_UpDirectionMatrix;

uniform sampler2D   _Azure_TransmittanceTexture;
uniform sampler3D   _Azure_InScatterTexture;
uniform float3      _Azure_EarthPosition;
uniform float       _Azure_PrecomputedSunIntensity, _Azure_PrecomputedMoonIntensity, _Azure_PrecomputedRg, _Azure_PrecomputedRt;

float3 AzureComputeStylizedFogScattering (float3 worldPos)
{
	// Initializations
    float3 transmittance = float3(1.0, 1.0, 1.0);
    
    // Directions
    float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos) * -1.0;
    float  sunCosTheta = dot(viewDir, _Azure_SunDirection);
    float  moonCosTheta = dot(viewDir, _Azure_MoonDirection);
    float  r = length(float3(0.0, 50.0, 0.0));
    float  sunRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_SunDirection) / r);
    float  moonRise = saturate(dot(float3(0.0, 500.0, 0.0), _Azure_MoonDirection) / r);

	// Optical depth
    //float zenith = acos(saturate(dot(float3(0.0, 1.0, 0.0), viewDir)));
    //float zenith = acos(length(viewDir.y));
    float zenith = acos(saturate(dot(float3(-1.0, 1.0, -1.0), length(viewDir))));
    float z = (cos(zenith) + 0.15 * pow(93.885 - ((zenith * 180.0f) / PI), -1.253));
    float SR = 8400.0 / z;
    float SM = 1200.0 / z;
    
    // Extinction
    float3 fex = exp(-(_Azure_Br*SR  + _Azure_Bm*SM));
    float  sunset = clamp(dot(float3(0.0, 1.0, 0.0), _Azure_SunDirection), 0.0, 0.5);
    if(_Azure_StylizedTransmittanceMode == 0)
    {
        transmittance = lerp(fex, (1.0 - fex), sunset);
        _Azure_TransmittanceColor = float3(1.0, 1.0, 1.0);
    }
    
    // Sun inScattering
    float  rayPhase = 2.0 + 0.5 * pow(sunCosTheta, 2.0);
    float  miePhase = MieG.x / pow(MieG.y - MieG.z * sunCosTheta, 1.5);
    
    float3 BrTheta  = Pi316 * _Azure_Br * rayPhase * _Azure_RayleighColor;
    float3 BmTheta  = Pi14  * _Azure_Bm * miePhase * _Azure_MieColor * sunRise;
    float3 BrmTheta = (BrTheta + BmTheta) * transmittance / (_Azure_Br + _Azure_Bm);
    
    float3 inScatter = BrmTheta * _Azure_TransmittanceColor * _Azure_ScatteringIntensity * (1.0 - fex);
    inScatter *= sunRise;
    
    // Moon inScattering
    rayPhase = 2.0 + 0.5 * pow(moonCosTheta, 2.0);
    miePhase = MieG.x / pow(MieG.y - MieG.z * moonCosTheta, 1.5);
    
    //BrTheta  = Pi316 * _Azure_Br * rayPhase * _Azure_RayleighColor;
    BmTheta  = Pi14  * _Azure_Bm * miePhase * _Azure_MieColor * moonRise;
    BrmTheta = (BrTheta + BmTheta) / (_Azure_Br + _Azure_Bm);
    
    float3 moonInScatter = BrmTheta * _Azure_TransmittanceColor * _Azure_ScatteringIntensity * 0.1 * (1.0 - fex);
    moonInScatter *= moonRise;
    moonInScatter *= 1.0 - sunRise;
    
    // Default night sky - When there is no moon in the sky
    BrmTheta = BrTheta / (_Azure_Br + _Azure_Bm);
    float3 skyLuminance = BrmTheta * _Azure_TransmittanceColor * _Azure_SkyLuminance * (1.0 - fex);

	// Output
    float3 OutputColor = inScatter + skyLuminance + moonInScatter;
    
    // Tonemapping
    OutputColor = saturate(1.0 - exp(-_Azure_Exposure * OutputColor));
    
    // Color correction
    OutputColor = pow(OutputColor, 2.2);
    #ifdef UNITY_COLORSPACE_GAMMA
    OutputColor = pow(OutputColor, 0.4545);
    #else
    OutputColor = OutputColor;
    #endif

	return OutputColor;
}


float SQRT(float f, float err)
{
    return f >= 0.0 ? sqrt(f) : err;
}

float inverseLerp(float min, float max, float t)
{
    return (t - min) / (max - min);
}

float4 texture4D(sampler3D table, float r, float mu, float muS, float nu)
{
    float H = sqrt(_Azure_PrecomputedRt * _Azure_PrecomputedRt - _Azure_PrecomputedRg * _Azure_PrecomputedRg);
    float rho = sqrt(r * r - _Azure_PrecomputedRg * _Azure_PrecomputedRg);
    float rmu = r * mu;
    float delta = rmu * rmu - r * r + _Azure_PrecomputedRg * _Azure_PrecomputedRg;
    float4 cst = rmu < 0.0 && delta > 0.0 ? float4(1.0, 0.0, 0.0, 0.5 - 0.5 / float(RES_MU)) : float4(-1.0, H * H, H, 0.5 + 0.5 / float(RES_MU));
    float uR = 0.5 / float(RES_R) + rho / H * (1.0 - 1.0 / float(RES_R));
    float uMu = cst.w + (rmu * cst.x + sqrt(delta + cst.y)) / (rho + cst.z) * (0.5 - 1.0 / float(RES_MU));
    // paper formula
    //float uMuS = 0.5 / float(RES_MU_S) + max((1.0 - exp(-3.0 * muS - 0.6)) / (1.0 - exp(-3.6)), 0.0) * (1.0 - 1.0 / float(RES_MU_S));
    // better formula
    float uMuS = 0.5 / float(RES_MU_S) + (atan(max(muS, -0.1975) * tan(1.26 * 1.1)) / 1.1 + (1.0 - 0.26)) * 0.5 * (1.0 - 1.0 / float(RES_MU_S));
    float _lerp = (nu + 1.0) / 2.0 * (float(RES_NU) - 1.0);
    float uNu = floor(_lerp);
    _lerp = _lerp - uNu;
    return tex3D(table, float3((uNu + uMuS) / float(RES_NU), uMu, uR)) * (1.0 - _lerp) +
           tex3D(table, float3((uNu + uMuS + 1.0) / float(RES_NU), uMu, uR)) * _lerp;
}

// Rayleigh phase function
float phaseFunctionR(float mu)
{
    return (3.0 / (16.0 * PI)) * (1.0 + mu * mu);
}

// Mie phase function
float phaseFunctionM(float mu)
{
    return 1.5 * 1.0 / (4.0 * PI) * (1.0 - mieG*mieG) * pow(1.0 + (mieG*mieG) - 2.0*mieG*mu, -3.0/2.0) * (1.0 + mu * mu) / (2.0 + mieG*mieG);
}

// Approximated single Mie scattering (cf. approximate Cm in paragraph "Angular precision")
float3 getMie(float4 rayMie)
{ 
    // rayMie.rgb=C*, rayMie.w=Cm,r
    return rayMie.rgb * rayMie.w / max(rayMie.r, 1e-4) * (betaR.r / betaR);
}

// Transmittance(=transparency) of atmosphere for infinite ray (r,mu)
// (mu=cos(view zenith angle)), intersections with ground ignored
float3 transmittance(float r, float mu)
{
    float uR, uMu;
    uR = sqrt((r - _Azure_PrecomputedRg) / (_Azure_PrecomputedRt - _Azure_PrecomputedRg));
    uMu = atan((mu + 0.15) / (1.0 + 0.15) * tan(1.5)) / 1.5;
    
    return tex2D(_Azure_TransmittanceTexture, float2(uMu, uR)).rgb;
}

// single scattered sunlight between two points
// camera=observer
// point=point on the ground
// sundir=unit vector towards the sun
// return scattered light and extinction coefficient
float3 inScattering(float3 camera, float3 _point, float3 sundir, out float3 extinction)
{
    float3 result;
    float3 viewdir = _point - camera;
    float d = length(viewdir);
    viewdir = viewdir / d;
    float r = length(camera);
    
    if (r < 0.9 * _Azure_PrecomputedRg)
    {
        camera += _Azure_EarthPosition;
        _point += _Azure_EarthPosition;
        r = length(camera);
    }
    float rMu = dot(camera, viewdir);
    float mu = rMu / r;
    float r0 = r;
    float mu0 = mu;

    float deltaSq = SQRT(rMu * rMu - r * r + _Azure_PrecomputedRt*_Azure_PrecomputedRt, 1e30);
    float din = max(-rMu - deltaSq, 0.0);
    if (din > 0.0 && din < d)
    {
        camera += din * viewdir;
        rMu += din;
        mu = rMu / _Azure_PrecomputedRt;
        r = _Azure_PrecomputedRt;
        d -= din;
    }

    if (r <= _Azure_PrecomputedRt)
    {
        float nu = dot(viewdir, sundir);
        float muS = dot(camera, sundir) / r;

        float4 inScatter;

        //if (r < _Azure_PrecomputedRg + 600.0)
        //{
        //    // avoids imprecision problems in aerial perspective near ground
        //    float f = (_Azure_PrecomputedRg + 600.0) / r;
        //    r = r * f;
        //    rMu = rMu * f;
        //    _point = _point * f;
        //}

        float r1 = length(_point);
        float rMu1 = dot(_point, viewdir);
        float mu1 = rMu1 / r1;
        float muS1 = dot(_point, sundir) / r1;

        if (mu > 0.0)
        {
            extinction = min(transmittance(r, mu) / transmittance(r1, mu1), 1.0);
        }
        else
        {
            extinction = min(transmittance(r1, -mu1) / transmittance(r, -mu), 1.0);
        }

        #ifdef HORIZON_HACK
        const float EPS = 0.004;
        float lim = -sqrt(1.0 - (_Azure_PrecomputedRg / r) * (_Azure_PrecomputedRg / r));
        if (abs(mu - lim) < EPS)
        {
            float a = ((mu - lim) + EPS) / (2.0 * EPS);

            mu = lim - EPS;
            r1 = sqrt(r * r + d * d + 2.0 * r * d * mu);
            mu1 = (r * mu + d) / r1;
            float4 inScatter0 = texture4D(_Azure_InScatterTexture, r, mu, muS, nu);
            float4 inScatter1 = texture4D(_Azure_InScatterTexture, r1, mu1, muS1, nu);
            float4 inScatterA = max(inScatter0 - inScatter1 * extinction.rgbr, 0.0);

            mu = lim + EPS;
            r1 = sqrt(r * r + d * d + 2.0 * r * d * mu);
            mu1 = (r * mu + d) / r1;
            inScatter0 = texture4D(_Azure_InScatterTexture, r, mu, muS, nu);
            inScatter1 = texture4D(_Azure_InScatterTexture, r1, mu1, muS1, nu);
            float4 inScatterB = max(inScatter0 - inScatter1 * extinction.rgbr, 0.0);

            inScatter = lerp(inScatterA, inScatterB, a);
        }
        else
        {
            float4 inScatter0 = texture4D(_Azure_InScatterTexture, r, mu, muS, nu);
            float4 inScatter1 = texture4D(_Azure_InScatterTexture, r1, mu1, muS1, nu);
            inScatter = max(inScatter0 - inScatter1 * extinction.rgbr, 0.0);
        }
        #else
        float4 inScatter0 = texture4D(_Azure_InScatterTexture, r, mu, muS, nu);
        float4 inScatter1 = texture4D(_Azure_InScatterTexture, r1, mu1, muS1, nu);
        inScatter = max(inScatter0 - inScatter1 * extinction.rgbr, 0.0);
        #endif

        //cancels inscatter when sun hidden by mountains
        //TODO smoothstep values depend on horizon angle in sun direction
        //inScatter.w *= smoothstep(0.035, 0.07, muS);

        // avoids imprecision problems in Mie scattering when sun is below horizon
        inScatter.w *= smoothstep(0.00, 0.02, muS);

        float3 inScatterM = getMie(inScatter);
        float phase = phaseFunctionR(nu);
        float phaseM = phaseFunctionM(nu);
        result = inScatter.rgb * _Azure_RayleighColor * phase + inScatterM * _Azure_MieColor * phaseM;
    }
    else
    {
        result = float3(0.0, 0.0, 0.0);
        extinction = float3(1.0, 1.0, 1.0);
    }

    return result;
}

// single scattered sunlight between two points
// camera=observer
// viewdir=unit vector towards observed point
// sundir=unit vector towards the sun
// return scattered light and extinction coefficient
float3 skyRadiance(float3 camera, float3 viewdir, float3 sundir, out float3 extinction)
{
    float3 result;
    camera += _Azure_EarthPosition;
    float r = length(camera);
    float rMu = dot(camera, viewdir);
    float mu = rMu / r;
    float r0 = r;
    float mu0 = mu;

    float deltaSq = SQRT(rMu * rMu - r * r + _Azure_PrecomputedRt*_Azure_PrecomputedRt, 1e30);
    float din = max(-rMu - deltaSq, 0.0);
    if (din > 0.0)
    {
        camera += din * viewdir;
        rMu += din;
        mu = rMu / _Azure_PrecomputedRt;
        r = _Azure_PrecomputedRt;
    }

    if (r <= _Azure_PrecomputedRt)
    {
        float nu = dot(viewdir, sundir);
        float muS = dot(camera, sundir) / r;

        float4 inScatter = texture4D(_Azure_InScatterTexture, r, rMu / r, muS, nu);
        inScatter *= min(transmittance(r, -mu) / transmittance(r0, -mu0), 1.0).rgbr;
        extinction = transmittance(r, mu);

        float3 inScatterM = getMie(inScatter);
        float phase = phaseFunctionR(nu);
        float phaseM = phaseFunctionM(nu);
        result = inScatter.rgb * _Azure_RayleighColor * phase + inScatterM * _Azure_MieColor * phaseM;
    } else
    {
        result = float3(0.0, 0.0, 0.0);
        extinction = float3(1.0, 1.0, 1.0);
    }

    return result;
}


float3 AzureComputePrecomputedFogScattering (float3 worldPos, float4 fragOutput)
{
    float3 extinction = float3(0.0, 0.0, 0.0);
    
    float3 viewDir = normalize(worldPos);
    //worldPos += _WorldSpaceCameraPos;
    //float d3 = distance(worldPos, _Azure_EarthPosition * -1.0);
    //if(dot(_WorldSpaceCameraPos, worldPos) < 0 && d3 >= _Azure_PrecomputedRt)
    //{
        //viewDir = normalize(worldPos - _WorldSpaceCameraPos);
    //}
    
    float height = distance(worldPos, _Azure_EarthPosition * -1.0);
    float atmFactor = saturate(inverseLerp(_Azure_PrecomputedRg, _Azure_PrecomputedRt, height));
    
    // InScattering:
    float3 sunInScatter = inScattering(_WorldSpaceCameraPos, worldPos, _Azure_SunDirection, extinction) * _Azure_PrecomputedSunIntensity;
    float3 moonInScatter = inScattering(_WorldSpaceCameraPos, worldPos, _Azure_MoonDirection, extinction) * _Azure_PrecomputedMoonIntensity;

    // Sky Radiance:
    float3 sunRadiance = skyRadiance(_WorldSpaceCameraPos, viewDir, _Azure_SunDirection, extinction) * _Azure_PrecomputedSunIntensity;
    float3 moonRadiance = skyRadiance(_WorldSpaceCameraPos, viewDir, _Azure_MoonDirection, extinction) * _Azure_PrecomputedMoonIntensity;
    
    // Output
    float3 OutputColor = fragOutput.rgb * extinction + lerp(sunInScatter + moonInScatter, sunRadiance + moonRadiance, atmFactor);
    
    // Tonemapping
    OutputColor = saturate(1.0 - exp(-_Azure_Exposure * OutputColor));
    
    // Color correction
    //OutputColor = pow(OutputColor, 2.2);
    #ifdef UNITY_COLORSPACE_GAMMA
    OutputColor = pow(OutputColor, 0.4545);
    #else
    OutputColor = OutputColor;
    #endif
    
    return OutputColor;
}