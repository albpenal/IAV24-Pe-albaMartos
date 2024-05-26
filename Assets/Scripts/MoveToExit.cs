using Pada1.BBCore.Tasks;
using Pada1.BBCore;
using UnityEngine;
using UnityEngine.AI;
using UCM.IAV.Movimiento;
using UCM.IAV.Navegacion;

namespace BBUnity.Actions
{
    /// <summary>
    /// It is an action to move the position of exit's GameObject.
    /// </summary>
    [Action("Navigation/MoveToExit")]
    [Help("Moves exit's GameObject")]
    public class MoveToExit : GOAction
    {
        ///<value>Reference to GraphGrid.</value>
        [InParam("GraphGrid")]
        [Help("Reference to GraphGrid")]
        public GraphGrid myGraphGrid;     

        /// <summary>Method to reset Exit's position.</summary>
        public override TaskStatus OnUpdate()
        {
            myGraphGrid.resetExit();
            return TaskStatus.COMPLETED;
        }
    }
}