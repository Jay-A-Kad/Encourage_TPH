/// <summary>
/// Represents the four power tiers of the Hype Meter.
/// The int value maps directly to the HypeState animator parameter.
/// </summary>
public enum HypeState
{
    Weak        = 0,   // 0–25 %   – trembling, crushers closing fast
    Struggling  = 1,   // 26–50 %  – barely holding back
    Strong      = 2,   // 51–75 %  – crushers nearly stopped
    Invincible  = 3    // 76–100 % – crushers pushed back, glowing aura
}
