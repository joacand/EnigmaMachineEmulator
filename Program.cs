using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma_Emulator {
    class Program {
        static void Main(string[] args) {
            EnigmaMachine machine = new EnigmaMachine();
            char[] rings = new char[] { 'Y', 'H', 'A' }; // Ringstellung - Ring settings
            char[] grund = new char[] { 'M', 'C', 'K' }; // Grundstellung - Initial rotor settings
            machine.setSettings(rings, grund);

            //Plugboard settings example
            machine.addPlug('K', 'H');

            // Message encrypt/decrypt example
            string msg = "ENIGMACRYPTEXAMPLE";
            Console.WriteLine("Original message: " + msg);
            string enc = machine.runEnigma(msg);
            Console.WriteLine("Encrypted: " + enc);

            // Reset the settings before decrypting!
            machine.setSettings(rings, grund);

            string dec = machine.runEnigma(enc);
            Console.WriteLine("Decrypted: " + dec);

            Console.ReadLine();
        }
    }
}
