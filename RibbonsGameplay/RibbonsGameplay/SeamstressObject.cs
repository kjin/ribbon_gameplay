using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace RibbonsGameplay
{
    public class SeamstressObject : BoxObject
    {

        #region Constants

            public const String SENSOR_NAME = "SeamstressGroundSensor";

        #endregion

        #region Properties

            /// <summary>
            /// Name of the ground sensor (used for Farseer)
            /// </summary>
            public String SensorName
            {
                get { return SENSOR_NAME; }
            }

        #endregion

        #region Fields

            // Ground sensor to represent feet
            private Fixture sensorFixture;

        #endregion

        #region Methods

            public SeamstressObject(World w)
            {

            }

            public override void Draw(GameCanvas g)
            {

            }

            public void OnGround()
            {

            }

            public void OffGround()
            {

            }

        #endregion

    }
}
