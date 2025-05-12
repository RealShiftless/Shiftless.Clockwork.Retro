using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Clockwork.Retro.Mathematics
{
    public class Time
    {
        // Values
        private float _deltaTime;
        private float _totalTime;

        private uint _curFrame;


        // Properties
        public float Delta => _deltaTime;
        public float Total => _totalTime;

        public uint Frame => _curFrame;


        // Func
        internal void Update(float dt)
        {
            _deltaTime = dt;
            _totalTime += dt;

            _curFrame++;
        }
    }
}
