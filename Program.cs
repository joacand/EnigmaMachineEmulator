using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma_Emulator {
    class Program {
        static void Main(string[] args) {
            EnigmaMachine machine = new EnigmaMachine();

            //Plugboard settings example
            machine.addPlug('k', 'h');
            machine.addPlug('a', 'e');
            machine.addPlug('r', 'y');
            machine.addPlug('c', 'm');
            machine.addPlug('q', 'x');

            // Message encrypt/decrypt example
            string msg = "kalle";
            Console.WriteLine("Original message: " + msg);
            string enc = machine.runEnigma(msg);
            string dec = machine.runEnigma(enc);
            Console.WriteLine("Encrypted: " + enc);
            Console.WriteLine("Decrypted: " + dec);
            Console.ReadLine();
        }
    }
}
