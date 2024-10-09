using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;

namespace ACT4
{
    public partial class Form1 : Form
    {
        int side;
        int n = 6;
        SixState startState;
        SixState currentState;
        int moveCounter;

        double temperature = 100.0;  // Initial temperature
        double coolingRate = 0.99;   // Cooling rate for temperature

        int[,] hTable;
        ArrayList possibleMoves;
        Random random = new Random();

        public Form1()
        {
            InitializeComponent();

            side = pictureBox1.Width / n;

            startState = randomSixState();
            currentState = new SixState(startState);

            updateUI();
            label1.Text = "Attacking pairs: " + getAttackingPairs(startState);
        }

        private void updateUI()
        {
            pictureBox2.Refresh();
            label3.Text = "Attacking pairs: " + getAttackingPairs(currentState);
            label4.Text = "Moves: " + moveCounter;

            hTable = getHeuristicTableForPossibleMoves(currentState);
            possibleMoves = getAllMoves(hTable);

            listBox1.Items.Clear();
            foreach (Point move in possibleMoves)
            {
                listBox1.Items.Add(move);
            }
        }

        private SixState randomSixState()
        {
            Random r = new Random();
            SixState random = new SixState(r.Next(n), r.Next(n), r.Next(n), r.Next(n), r.Next(n), r.Next(n));
            return random;
        }

        private int getAttackingPairs(SixState f)
        {
            int attackers = 0;
            for (int rf = 0; rf < n; rf++)
            {
                for (int tar = rf + 1; tar < n; tar++)
                {
                    if (f.Y[rf] == f.Y[tar]) attackers++; // Horizontal
                    if (f.Y[tar] == f.Y[rf] + tar - rf) attackers++; // Diagonal down
                    if (f.Y[rf] == f.Y[tar] + tar - rf) attackers++; // Diagonal up
                }
            }
            return attackers;
        }

        private int[,] getHeuristicTableForPossibleMoves(SixState thisState)
        {
            int[,] hStates = new int[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    SixState possible = new SixState(thisState);
                    possible.Y[i] = j;
                    hStates[i, j] = getAttackingPairs(possible);
                }
            }
            return hStates;
        }

        private ArrayList getAllMoves(int[,] heuristicTable)
        {
            ArrayList allMoves = new ArrayList();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (currentState.Y[i] != j)
                        allMoves.Add(new Point(i, j));
                }
            }
            return allMoves;
        }

        private void executeMove(Point move)
        {
            currentState.Y[move.X] = move.Y;
            moveCounter++;
            updateUI();
        }

        private void SimulatedAnnealing()
        {
            while (temperature > 1 && getAttackingPairs(currentState) > 0)
            {
                ArrayList neighbors = getAllMoves(hTable);
                Point nextMove = (Point)neighbors[random.Next(neighbors.Count)];
                SixState nextState = new SixState(currentState);
                nextState.Y[nextMove.X] = nextMove.Y;

                int currentEnergy = getAttackingPairs(currentState);
                int nextEnergy = getAttackingPairs(nextState);
                int energyDifference = nextEnergy - currentEnergy;

                if (energyDifference < 0 || Math.Exp(-energyDifference / temperature) > random.NextDouble())
                {
                    executeMove(nextMove);  // Accept the move
                    currentState = nextState;
                }

                temperature *= coolingRate;  // Cool down
                updateUI();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SimulatedAnnealing();  // Start simulated annealing on button click
        }

        private void button3_Click(object sender, EventArgs e)
        {
            startState = randomSixState();
            currentState = new SixState(startState);
            temperature = 100.0; // Reset temperature
            moveCounter = 0;
            updateUI();
            pictureBox1.Refresh();
            label1.Text = "Attacking pairs: " + getAttackingPairs(startState);
        }
    }

}
