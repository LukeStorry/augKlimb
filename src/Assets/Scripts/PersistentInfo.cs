using System.Collections.Generic;

public static class PersistentInfo
{
    public static List<ClimbData> climbs = FileHandler.LoadClimbs();
    public static ClimbData currentClimb = climbs[0];
}
