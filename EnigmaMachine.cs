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
        private Rotor reflector;

        string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";


        class Rotor {
            // The current char of the alphabet, and position of it. This char is visible outside the machine
            public int outerPosition;
            char outerChar;

            // The fixed alphabet of the rotor
            string wiring;

            // turnOver is the notch on which letter the rotors turnover point is
            char turnOver;

            // Ring is the wiring setting relative to the turnover notch and position
            // Basically part of the initialization vector
            public char ring;

            public int[] map = new int[26];
            public int[] revMap = new int[26];

            public Rotor(string w, char to) {
                wiring = w;
                turnOver = to;
                outerPosition = 0;
                outerChar = wiring.ToCharArray()[outerPosition];
                ring = 'A'; // A default ring setting

                // Fill the mapping arrays
                for (int i = 0; i < 26; i++) {
                    int match = ((int)wiring.ToCharArray()[i]) - 65;
                    map[i] = (26 + match - i) % 26;
                    revMap[match] = (26 + i - match) % 26;
                }
            }

            public void setRing(char r) {
                ring = r;
            }

            public void setOuterPosition(int i) {
                outerPosition = i;
                outerChar = wiring.ToCharArray()[outerPosition];
            }

            public void setOuterChar(char c) {
                outerChar = c;
                outerPosition = wiring.IndexOf(outerChar);
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

        // Apply the rotor scramble to character using all three rotors
        // Argumentent reverse decides which direction we are scrambling
        private char rotorMap(char c, bool reverse) {
            int cPos = (int)c - 65;
            if (reverse) {
                int rPos = (int)rI.ring - 65;
                int d = rI.revMap[(26 + cPos + rI.outerPosition - rPos) % 26];
                cPos = (cPos + d) % 26;

                rPos = (int)rII.ring - 65;
                d = rII.revMap[(26 + cPos + rII.outerPosition - rPos) % 26];
                cPos = (cPos + d) % 26;

                rPos = (int)rIII.ring - 65;
                d = rIII.revMap[(26 + cPos + rIII.outerPosition - rPos) % 26];
                cPos = (cPos + d) % 26;
            } else {
                int rPos = (int)rIII.ring - 65;
                int d = rIII.map[(26 + cPos + rIII.outerPosition - rPos) % 26];
                cPos = (cPos + d) % 26;

                rPos = (int)rII.ring - 65;
                d = rII.map[(26 + cPos + rII.outerPosition - rPos) % 26];
                cPos = (cPos + d) % 26;

                rPos = (int)rI.ring - 65;
                d = rI.map[(26 + cPos + rI.outerPosition - rPos) % 26];
                cPos = (cPos + d) % 26;
            }

            return alphabet.ToCharArray()[cPos];
        }

        // Apply the reflector, the part that comes after the rotors
        private char reflectorMap(char c) {
            int cPos = (int)c - 65;
            cPos = (cPos + reflector.map[cPos]) % 26;
            return alphabet.ToCharArray()[cPos];
        }

        // Constructor
        public EnigmaMachine() {
            plugBoard = new Dictionary<char, char>();

            // Notch and alphabet are fixed on the rotor
            // First argument is alphabet, second is the turnover notch
            rI = new Rotor("EKMFLGDQVZNTOWYHXUSPAIBRCJ", 'Q');
            rII = new Rotor("AJDKSIRUXBLHWTMCQGZNPYFVOE", 'E');
            rIII = new Rotor("BDFHJLCPRTXVZNYEIWGAKMUSQO", 'V');
            reflector = new Rotor("YRUHQSLDPXNGOKMIEBFZCWVJAT", 'B');
        }

        // Enter the ring settings and initial rotor positions
        public void setSettings(char[] rings, char[] grund) {
            rI.setRing(rings[0]);
            rII.setRing(rings[1]);
            rIII.setRing(rings[2]);

            rI.setOuterChar(grund[0]);
            rII.setOuterChar(grund[1]);
            rIII.setOuterChar(grund[2]);
        }

        // Encrypts or decrypts a message
        public string runEnigma(string msg) {
            StringBuilder encryptedMessage = new StringBuilder();

            msg = msg.ToUpper();

            foreach (char c in msg) {
                encryptedMessage.Append(encryptChar(c));
            }

            return encryptedMessage.ToString();
        }

        // Encrypts (or decrypts) a single character
        private char encryptChar(char c) {

            // Rotate the rotors before scrambling
            rotateRotors(rI, rII, rIII);

            // Into plugboard from keyboard <--
            if (plugBoard.ContainsKey(c)) {
                c = plugBoard[c];
            }

            // Scramble with rotors

            // First we go all the way through the rotors <--
            c = rotorMap(c, true);

            // Reflect at the end so we don't just unscramble it again when we go back
            c = reflectorMap(c);

            // Go back through all the rotors the other way -->
            c = rotorMap(c, false);

            // Plugboard again, from other direction -->
            if (plugBoard.ContainsKey(c)) {
                c = plugBoard[c];
            }

            // Character is now encrypted
            return c;
        }

        // Add a character pair into the plugboard
        public void addPlug(char c, char cc) {
            if (c != cc && !plugBoard.ContainsKey(c)) {
                plugBoard.Add(c, cc);
                plugBoard.Add(cc, c);
            }
        }
    }
}
