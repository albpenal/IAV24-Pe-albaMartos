using Pada1.BBCore.Framework;
using Pada1.BBCore;
using UnityEngine;
using UCM.IAV.Navegacion;

namespace BBCore.Conditions
{
    /// <summary>
    /// It is a basic condition to check if exit is save.
    /// </summary>
    [Condition("Basic/CheckExit")]
    [Help("Checks if exit is save")]
    public class CheckExit : ConditionBase
    {
        ///<value>Input First Boolean Parameter.</value>
        [InParam("GraphGrid")]
        [Help("Reference to GraphGrid")]
        public GraphGrid myGraphGrid;


        /// <summary>
        /// Checks whether exit is save.
        /// </summary>
        /// <returns>true if the exit is save, false if it is not.</returns>
        public override bool Check()
        {
            Debug.Log(myGraphGrid.salidaSave());
            return myGraphGrid.salidaSave();
        }
    }
}