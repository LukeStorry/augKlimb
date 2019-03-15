using System.Collections.Generic;

public static class PersistentInfo
{
    public static List<ClimbData> _climbs;
    public static ClimbData _currentClimb;

    // Stores the climb that ViewClimb will detail
    public static ClimbData CurrentClimb
    {
        get { return _currentClimb ?? Climbs[0]; } // backup, so ViewClimb can still be run in Editor
        set { _currentClimb = value; }
    }

    // A cached list of all climbs
    public static List<ClimbData> Climbs
    {
        get
        {
            if (_climbs == null) _climbs = FileHandler.LoadClimbs(); // Initialisation
            return _climbs;
        }
    }
}
