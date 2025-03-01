using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LifePointsTCG
{
    class Monster
    {
        public int vida_original {  get; set; }
        public int vida { get; set; }
        public bool exists {  get; set; }
        public int damageRecived { get; set; }
        public int estado {  get; set; }
        public int isPoisoned { get; set; }
        public int isBurned { get; set; }
        
        public Monster(int vida_original) 
        {
            damageRecived = 0;
            this.vida_original = vida_original;
            this.vida = vida_original;
            exists = false;
            estado = 0;
            isPoisoned = 0;
            isBurned = 0;
        }
    }
}
