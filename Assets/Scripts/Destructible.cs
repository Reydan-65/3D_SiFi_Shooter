using System.Collections.Generic;
using UnityEngine;

public class Destructible : DestructibleBase
{
    public static Destructible FindNearest(Vector3 position)
    {
        float minDist = float.MaxValue;
        Destructible target = null;

        foreach (Destructible destructible in m_AllDestructible)
        {
            float curDist = Vector3.Distance(destructible.transform.position, position);

            if (curDist < minDist)
            {
                minDist = curDist;
                target = destructible;
            }
        }

        return target;
    }

    public static Destructible FindNearestNonTeamMember(Destructible destructible)
    {
        float minDist = float.MaxValue;
        Destructible target = null;

        foreach (Destructible dest in m_AllDestructible)
        {
            if (dest.IsDead) continue;

            float curDist = Vector3.Distance(dest.transform.position, destructible.transform.position);

            if (curDist < minDist && destructible.TeamId != dest.TeamId)
            {
                minDist = curDist;
                target = dest;
            }
        }

        return target;
    }

    public static List<Destructible> GetAllTeamMembers(int teamID)
    {
        List<Destructible> teamDestructble = new List<Destructible>();

        foreach (Destructible destructible in m_AllDestructible)
        {
            if (destructible.TeamId == teamID)
            {
                teamDestructble.Add(destructible);
            }
        }

        return teamDestructble;
    }

    public static List<Destructible> GetAllNonTeamMembers(int teamID)
    {
        List<Destructible> nonTeamDestructble = new List<Destructible>();

        foreach (Destructible destructible in m_AllDestructible)
        {
            if (destructible.TeamId != teamID)
            {
                nonTeamDestructble.Add(destructible);
            }
        }

        return nonTeamDestructble;
    }
}
