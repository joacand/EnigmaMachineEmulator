using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma_Emulator {
    class EnigmaMachine {
        /* Enigma Machine
           Modelled after Enigma I, from ~1930
           Work in progress
        */
        private Dictionary<Char, Char> plugBoard;

        private Rotor rI;
        private Rotor rII;
        private Rotor rIII;

        class Rotor {
            int outerPosition;
            char outerChar;
            string wiring;
            char turnOver;

            public Rotor(string w, char to) {
                wiring = w;
                turnOver = to;
                outerPosition = 0;
                outerChar = wiring.ToCharArray()[outerPosition];
            }

            public void setOuterPosition(int i) {
                outerPosition = i;
                outerChar = wiring.ToCharArray()[outerPosition];
            }

            public void step() {
                outerPosition = (outerPosition + 1) % 26;
                outerChar = wiring.ToCharArray()[outerPosition];
            }

            public char getOuterChar() {
                return outerChar;
            }

            public bool isInTurnOver() {
                return outerChar == turnOver;
            }
        }

        private void rotateRotors(Rotor I, Rotor II, Rotor III) {
            if (II.isInTurnOver()) {
                // If rotor II is on turnOver, all rotors step
                I.step();
                II.step();
            } else if (III.isInTurnOver()) {
                // If rotor III is on turnOver, the two rotors to the right step
                II.step();
            }

            // Rotor III always steps
            III.step();
        }

        public EnigmaMachine() {
            plugBoard = new Dictionary<char, char>();
            rI = new Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
            rII = new Rotor("AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
            rIII = new Rotor("BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
        }

        public string runEnigma(string msg) {
            StringBuilder encryptedMessage = new StringBuilder();

            foreach (char c in msg) {
                encryptedMessage.Append(encryptChar(c));
            }

            return encryptedMessage.ToString();
        }

        private char encryptChar(char c) {
            // Plugboard
            if (plugBoard.ContainsKey(c)) {
                c = plugBoard[c];
            }

            // Rotate the rotors before scrambling
            rotateRotors(rI, rII, rIII);

            // Static rotor here - does not change the signal
            // Scramble with rotors
            // TODO

            return c;
        }

        public void addPlug(char c, char cc) {
            if (c != cc) {
                plugBoard.Add(c, cc);
                plugBoard.Add(cc, c);
            }
        }
    }
}
