using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recellection
{
    /* Contains information about traits of different Terrain Types.
     * 
     * Author: Joel Ahlgren
     * Date: 2010-04-11
     */
    class TerrainType
    {
        /*
         * Data
         */
        private int dmgMod; // Damage modifier
        private int spdMod; // Speed modifier
        private int rscMod; // Resource modifier

        /*
         * Methods
         */
        public TerrainType()
        {
            // Assume default type.
            this.dmgMod = 0;
            this.spdMod = 10;
            this.rscMod = 10;
        }
        public TerrainType(Globals.TerrainTypes t)
        {
            switch (t)
            {
                case(Globals.TerrainTypes.Membrane):
                {
                    this.dmgMod = 0;
                    this.spdMod = 10;
                    this.rscMod = 10;
                    break;
                }
                case (Globals.TerrainTypes.Mucus):
                {
                    this.dmgMod = 0;
                    this.spdMod = 8;
                    this.rscMod = 12;
                    break;
                }
                case (Globals.TerrainTypes.Slow):
                {
                    this.dmgMod = 0;
                    this.spdMod = 5;
                    this.rscMod = 10;
                    break;
                }
                case (Globals.TerrainTypes.Infected):
                {
                    this.dmgMod = 5;
                    this.spdMod = 10;
                    this.rscMod = 5;
                    break;
                }
            }
        }
    }
}
