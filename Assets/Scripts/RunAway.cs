using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using UnityEngine.AI;
using UCM.IAV.Movimiento;
using UCM.IAV.Navegacion;

namespace BBUnity.Actions
{
    /// <summary>
    /// It is an action to move the position of exit's GameObject to a save cell.
    /// </summary>
    [Action("Navigation/RunAway")]
    [Help("Moves exit's GameObject into a save cell")]
    public class RunAway : GOAction
    {
        ///<value>Reference to GraphGrid.</value>
        [InParam("GraphGrid")]
        [Help("Reference to GraphGrid")]
        public GraphGrid myGraphGrid;

        /// <summary>Method to move Exit's position.</summary>
        public override TaskStatus OnUpdate()
        {
            myGraphGrid.setExit();
            return TaskStatus.COMPLETED;
        }
    }
}