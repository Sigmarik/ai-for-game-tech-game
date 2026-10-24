
using System.Collections.Generic;
using UnityEngine.Playables;

public static class AITaskCoordinator
{
    /// <summary>
    ///  This class this handle how AI coorporate with other  AIs or Player - record which task was taken by player or AI
    /// </summary>
    /// 
    
    static Dictionary<Task, object> claims = new Dictionary<Task, object>();

    public static bool TryClaimTask(Task task, object owner)// this method is for AI
    {
        object currentTaskOwner;
        if(claims.TryGetValue(task, out currentTaskOwner)) // task is occupied
        {
            if(currentTaskOwner != owner) // task is occupied by others
                return false;
        }
        claims[task] = owner;
        return true;
    }

    public static void ForceClaimTask(Task task, object owner) // this method is for player
    {
        claims[task] = owner;
    }

    public static bool IsClaimByOthers(Task task, object owner)
    {
        object currentTaskOwner;
        if(claims.TryGetValue(task, out currentTaskOwner)) // task is occupied
        {
            if(currentTaskOwner != owner) // task is occupied by others
                return true;
        }
        return false;

    }
    
    public static void ReleaseTask(Task task, object owner)
    {
        object currentTaskOwner;
        if (claims.TryGetValue(task, out currentTaskOwner) && currentTaskOwner == owner)
            claims.Remove(task);
    }
}
