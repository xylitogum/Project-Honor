using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.FSM_Strategic;
using Tree;
using UnityEditor;
using UnityEngine;

/// <summary>
/// The manager responsible for relaying current
/// strategy to each member of the strategic team
/// through the team hierarchy
/// </summary>
public class CommandPassingManager
{
    private Team StrategicTeam;
    private Commander_FSM Commander;

	// Use this for initialization
	public void Start ()
	{
	    StrategicTeam = GameManager.instance.teams[0];
	    Commander = GameManager.instance.commander;
	    foreach (var strategicTeamMember in StrategicTeam.members)
	    {
	        strategicTeamMember.strategicOrders = new StrategicCommand(
                GameManager.instance.commander.strategyFSM.CurrentState,
                GameManager.instance.commander.teamTarget);
	    }
    }
	
	// Update is called once per frame
	public void Update () {
        // Give the commander at the top of the hierarchy the current strategy
        StrategicTeam.captain.strategicOrders = new StrategicCommand(Commander.strategyFSM.CurrentState, Commander.teamTarget);
		BfsCommandPassing();
    }

    /// <summary>
    /// Performs breadth-first search of the team hierarchy tree
    /// and passes the current strategy between units.
    /// </summary>
    private void BfsCommandPassing()
    {
        List<TreeNode<Character>> searched = new List<TreeNode<Character>>();
        Queue<TreeNode<Character>> queue = new Queue<TreeNode<Character>>();

        searched.Add(StrategicTeam.teamHierarchy);
        queue.Enqueue(StrategicTeam.teamHierarchy);

        while (queue.Count != 0)
        {
            var current = queue.Dequeue();
            foreach (var child in current.Children)
            {
                var commandToPass = current.Value != null ? current.Value.strategicOrders : 
                        new StrategicCommand(Commander.strategyFSM.CurrentState, Commander.teamTarget);

                if (!searched.Contains(child))
                {
                    // Pass this agent's command onto its child
                    if (child.Value != null)
                    {
                        child.Value.strategicOrders = commandToPass;
                    }
                    searched.Add(child);
                    queue.Enqueue(child);
                }
            }
        }
    }
}
