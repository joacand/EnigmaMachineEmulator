using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma_Emulator {
    class Program {
        static void Main(string[] args) {
            EnigmaMachine machine = new EnigmaMachine();
            char[] rings = new char[] { 'V', 'E', 'S' }; // Ringstellung - Ring settings
            char[] grund = new char[] { 'F', 'V', 'E' }; // Grundstellung - Initial rotor settings
            string order = "I-II-III"; // Ordering of rotors. Examples: "I-II-III", "II-I-III", "III-II-I"
            machine.setSettings(rings, grund, order);

            //Plugboard settings example
            machine.addPlug('K', 'H');


            // Message encrypt/decrypt example
            string msg = "ENIGMACRYPTEXAMPLE";
            Console.WriteLine("Plain text:\t" + msg);
            string enc = machine.runEnigma(msg);
            Console.WriteLine("Encrypted:\t" + enc);

            // Reset the settings before decrypting!
            machine.setSettings(rings, grund, order);

            string dec = machine.runEnigma(enc);
            Console.WriteLine("Decrypted:\t" + dec);

            Console.ReadLine();
        }
    }
}
