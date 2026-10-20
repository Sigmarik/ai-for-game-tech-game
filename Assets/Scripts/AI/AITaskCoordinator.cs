
using System.Collections.Generic;

public static class AITaskCoordinator
{
    /// <summary>
    ///  This class this handle how AI coorporate with other  AIs or Player - record which task was taken by player or AI
    /// </summary>
    /// 
    
    static Dictionary<Task, AIBrain> claims = new Dictionary<Task, AIBrain>();

    public static bool TryClaimTask(Task task, AIBrain owner)
    {
        AIBrain currentTaskOwner;
        if(claims.TryGetValue(task, out currentTaskOwner)) // task is occupied
        {
            if(currentTaskOwner != owner) // task is occupied by others
                return false;
        }
        claims[task] = owner;
        return true;
    }

    public static bool IsClaimByOthers(Task task, AIBrain owner)
    {
        AIBrain currentTaskOwner;
        if(claims.TryGetValue(task, out currentTaskOwner)) // task is occupied
        {
            if(currentTaskOwner != owner) // task is occupied by others
                return true;
        }
        return false;

    }
    
    public static void ReleaseTask(Task task, AIBrain owner)
    {
        AIBrain currentTaskOwner;
        if (claims.TryGetValue(task, out currentTaskOwner) && currentTaskOwner == owner)
            claims.Remove(task);
    }
}
