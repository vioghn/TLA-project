using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Security.AccessControl;
using System.Globalization;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.Threading;


namespace WindowsFormsApp1
{
    static class Program
    {
         /// <summary>
         /// The main entry point for the application.
        /// </summary>
         [STAThread]
        public static void Main(string[] args)
        {


            //Console.WriteLine();n
            // making HashSet of states
            string[] s1 = Console.ReadLine().Split();
            List<State> states = new List<State>();
            Dictionary<string, State> states2 = new Dictionary<string, State>();
            for (int i = 0; i < s1.Length; i++)
            {
                State q = new State(s1[i]);
                states.Add(q);
            }
            states[0].isFirst = true;
            //get alphabet
            string[] alphabet = Console.ReadLine().Split();
            //get finish states
            string[] finishStates = Console.ReadLine().Split();
            HashSet<string> final = new HashSet<string>();
            HashSet<State> f = new HashSet<State>();
            for (int j = 0; j < finishStates.Length; j++)
            {
                foreach (var q in states)
                {
                    if (q.value == finishStates[j])
                        q.isFinish = true;
                }
            }
            foreach (var aa in finishStates)
                final.Add(aa);
            for (int i = 0; i < s1.Length; i++)
            {
                State q = new State(s1[i]);

                if (final.Contains(q.value))
                {
                    q.isFinish = true;
                    f.Add(q);
                }

                states2.Add(s1[i], q);
            }
            states2["q0"].isFirst = true;
            //add transitions to states
            int numOfRules = int.Parse(Console.ReadLine());
            for (int k = 0; k < numOfRules; k++)
            {
                List<string> transition = Console.ReadLine().Split().ToList();
                if (transition.Count == 2)
                    transition.Add("");
                foreach (var q in states)
                {
                    if (q.value == transition[0])
                    {
                        foreach (var destination in states)
                        {
                            if (destination.value == transition[1])
                                q.addTransition(destination, transition[2]);

                        }
                    }
                }
                states2[transition[0]].addTransition(states2[transition[1]], transition[2]);
            }

            NFA d2 = new NFA(states);
            // DFA d = new DFA(states2 , f, alphabet); 
            // d.minimization();
            // d2.createEquivalentDFA(alphabet);
            var x = d2.convertodfa(alphabet);
            // x.minimization();
            var d = x.minimization();
            Console.Write(d2.isAcceptByNFA("", numOfRules));
            Console.Write(d2.isAcceptByNFA("abb", numOfRules));
            Console.Write(d2.isAcceptByNFA("abaa", numOfRules));
            Console.Write(d2.isAcceptByNFA("abab", numOfRules));
            Console.Write(d2.isAcceptByNFA("aaaaaa", numOfRules));





            Console.WriteLine("-----------------------------");
            Console.WriteLine(x.isAcceptByDFA(""));

            Console.WriteLine(x.isAcceptByDFA("abb"));

            Console.WriteLine(x.isAcceptByDFA("abaa"));

            Console.WriteLine(x.isAcceptByDFA("abab"));

            Console.WriteLine(x.isAcceptByDFA("aaaaaa"));
            //d2.ShowNFa();
            // x.ShowDFa();





        }
    }
    class State
    {
        public string value;
        public bool isFirst = false;
        public bool isFinish = false;
        public bool isvisited = false;
        public bool iscombined = false;

        public HashSet<Tuple<string, State>> nextState;
        public List<State> subStates = new List<State>();
        public int Id;
        public State(string val)
        {
            this.value = val;
            this.nextState = new HashSet<Tuple<string, State>>();
        }
        public void addTransition(State q, string alph)
        {
            Tuple<string, State> t = new Tuple<string, State>(alph, q);
            nextState.Add(t);
        }
        public override bool Equals(object obj)
        {
            if (obj is State)
            {
                return this.value == (obj as State).value;
            }
            else
                return false;
        }
        public override int GetHashCode() => value.GetHashCode();
    }

    class NFA
    {
        public List<State> states = new List<State>();
        public HashSet<State> finals = new HashSet<State>();
        public HashSet<State> starts = new HashSet<State>();
        //  int transitioncount; 
        public NFA(List<State> givenStates)
        {
            this.states = givenStates;
        }

        public bool isAcceptByNFA(string s, int trc)
        {
            int howmuch = 0;
            bool[] isvisited = new bool[s.Length];
            //store the positions and the current state
            Stack<Tuple<State, int>> visit = new Stack<Tuple<State, int>>();
            //current position
            //ab 
            int count = -1;
            int id = -1;
            int id2 = -1;
            visit.Push(new Tuple<State, int>(states[0], count));
            while (visit.Count != 0)
            {

                var temp = visit.Pop();
                id = temp.Item2;
                id2 = id + 1;
                foreach (var x in temp.Item1.nextState)
                {
                    if (id2 < s.Length)
                    {
                        if (x.Item1 == s[id2].ToString())
                        {
                            visit.Push(new Tuple<State, int>(x.Item2, id2));
                            isvisited[id2] = true;

                        }
                    }
                    //how to fix loops by landa =) 
                    if (x.Item1 == "")
                        visit.Push(new Tuple<State, int>(x.Item2, id));


                }
                if (temp.Item1.isFinish && temp.Item2 == s.Length - 1)
                {
                    return true;
                }

                //?
                //with q.count 
                if (howmuch > 2 * trc * trc && howmuch >= s.Length)
                {
                    foreach (var x in isvisited)
                    {
                        if (!x)
                            return false;

                    }
                }
                Console.WriteLine(visit.Count);
                howmuch++;



            }
            return false;
        }

        public Tuple<HashSet<State>, HashSet<State>> createEquivalentDFA(string[] alphabet)
        {
            HashSet<State> newfinal = new HashSet<State>();
            State Tohi = new State("null");
            Tohi.Id = -1;
            for (int i1 = 0; i1 < alphabet.Length; i1++)
            {
                Tuple<string, State> n = new Tuple<string, State>(alphabet[i1], Tohi);
                Tohi.nextState.Add(n);
            }
            HashSet<State> DFAStatea = new HashSet<State>();
            Queue<State> outS = new Queue<State>();
            Queue<State> nulltr = new Queue<State>();

            ///add null values to the nulltr;
            nulltr.Enqueue(states[0]);

            for (int i = 0; i < states.Count; i++)
            {
                nulltr.Clear();
                nulltr.Enqueue(states[i]);
                for (int j = 0; j < states.Count; j++)
                {
                    states[j].iscombined = false;
                }
                while (nulltr.Count != 0)
                {
                    var p = nulltr.Dequeue();
                    p.iscombined = true;
                    foreach (var n in p.nextState)
                    {
                        if (n.Item1 == "")
                            if (!p.subStates.Contains(n.Item2))
                                p.subStates.Add(n.Item2);

                        if (!n.Item2.iscombined)
                            nulltr.Enqueue(n.Item2);
                    }
                }
            }


            for (int i = 0; i < states.Count; i++)
            {
                var s = states[i];
                nulltr.Clear();
                for (int j = 0; j < states.Count; j++)
                {
                    states[j].isvisited = false;
                }
                nulltr.Enqueue(s);
                List<State> a = new List<State>();
                while (nulltr.Count != 0)
                {
                    s = nulltr.Dequeue();
                    s.isvisited = true;
                    foreach (var ss in s.subStates)
                    {
                        a.Add(ss);
                        if (!ss.isvisited)
                            nulltr.Enqueue(ss);
                    }
                }
                foreach (var u in a)
                    if (!states[i].subStates.Contains(u))
                        states[i].subStates.Add(u);

            }
            State q0 = new State("q0");
            q0.isFirst = true;
            q0.subStates.Add(states[0]);
            foreach (var ss in states[0].subStates)
            {
                if (!q0.subStates.Contains(ss))
                {
                    q0.subStates.Add(ss);
                }
            }
            q0.Id = 1;
            outS.Enqueue(q0);
            DFAStatea.Add(q0);


            HashSet<State> subNext1 = new HashSet<State>();
            HashSet<State> subNext2 = new HashSet<State>();


            while (outS.Count > 0)
            {
                State temp = outS.Dequeue();
                foreach (var alph in alphabet)
                {
                    for (int i = 0; i < temp.subStates.Count(); i++)
                    {
                        subNext1 = temp.subStates[i].nextState.Where(tuple => tuple.Item1 == alph)
                        .Select(tuple => tuple.Item2).ToHashSet();

                        foreach (var item in subNext1)
                        {
                            subNext2.Add(item);
                            if (item.subStates.Count != 0)
                                foreach (var xx in item.subStates)
                                {
                                    subNext2.Add(xx);
                                    if (xx.isFinish == true)
                                    {
                                        temp.isFinish = true;
                                    }
                                }

                        }
                        subNext1 = null;
                    }
                    if (subNext2.Count == 0)
                    {
                        temp.addTransition(Tohi, alph);
                        DFAStatea.Add(Tohi);
                    }
                    else
                    {
                        int id = makeId(subNext2);
                        if (DFAStatea.Where(q => q.Id == id).ToHashSet().Count() == 0)
                        {
                            State mix = new State($"{id}");
                            mix.Id = id;
                            mix.subStates = subNext2.ToList();
                            if (subNext2.Where(q => finals.Contains(q) || q.isFinish == true).ToHashSet().Count() != 0)
                            {
                                mix.isFinish = true;
                                finals.Add(mix);


                            }
                            // if(temp.isFirst == true)
                            // {
                            //     mix.isFirst = true;
                            // }


                            DFAStatea.Add(mix);
                            outS.Enqueue(mix);
                            temp.addTransition(mix, alph);

                        }
                        else
                        {
                            temp.addTransition(DFAStatea.Where(q => q.Id == id).Select(q => q).First(), alph);
                        }

                    }
                    subNext2.Clear();

                }
                //add  all null substate --> with bfs with another function 
                //run the null function first 
                //do the normal way in order =) 


            }

            //schematic for me
            foreach (var item in DFAStatea)
            {
                // Console.WriteLine($"{item.value} ");
                // Console.WriteLine(item.isFinish);

                foreach (var item1 in item.nextState)
                {
                    // Console.WriteLine(item1.Item1);
                    // Console.WriteLine(item1.Item2.value);
                    Console.WriteLine($"finished:({item.isFinish}) :{item.value}={item1.Item1} ==> {item1.Item2.value}");
                }
                // Console.WriteLine("-----------------------");
            }
            // Console.ReadLine();
            newfinal = finals;
            newfinal.IntersectWith(DFAStatea);
            foreach (var f in newfinal)
            {
                Console.Write(f.value + " ");
            }
            return new Tuple<HashSet<State>, HashSet<State>>(DFAStatea, newfinal);

        }

        public int makeId(HashSet<State> subNext2)
        {
            int sum = 0;
            foreach (var item in subNext2)
            {
                sum += (int)Math.Pow(2, int.Parse(item.value.Last().ToString()));
            }
            return sum;
        }



        //
        //      public string NfaToReg(string[] alphabet)
        //        {
        //            List<List<string>> L = new List<List<string>>();
        //            for(int p1 =0 ; p1 <states.Count ; p1++)
        //            {   List<string> s = new List<string>();
        //                for(int p2 =0 ; p2 <states.Count ; p2++)
        //                {
        //                    s.Add("");
        //                }
        //                L.Add(s);
        //            }
        //            
        //            for(int i =0 ; i<states.Count ; i++)
        //            {
        //                for(int j =0 ; j <states.Count ; j++)
        //                {
        //                    foreach (var alph in alphabet)
        //                    {
        //                        if(states[i].nextState.Where(tuple => tuple.Item1 == alph).Select(tuple => tuple.Item2).Contains(states[j]))
        //                        {
        //                            if(L[i][j] == "")
        //                                L[i][j] +=$"{alph}" ;
        //                            else    
        //                                L[i][j] +=$"+{alph}" ;
        //                        }
        //                        
        //                    }
        //                }
        //            }
        //                for(int k=0 ;k <states.Count ; k++)
        //                {
        //                    for(int i1=0 ; i1<states.Count ; i1++)
        //                    {
        //                        for(int j1=0 ; j1 <states.Count ; j1++)
        //                        {
        //                            L[i1][i1] += $" {L[i1][k]} . {star(L[k][k])} . {L[k][i1]} ";
        //                            L[j1][j1] += $" {L[j1][k]} . {star(L[k][k])} . {L[k][j1]} ";
        //                            L[i1][j1] += $" {L[i1][k]} . {star(L[k][k])} . {L[k][j1]} ";
        //                            L[j1][i1] += $" {L[j1][k]} . {star(L[k][k])} . {L[k][i1]} ";
        //                        
        //                        }
        //                    }   
        //                }
        //                
        //                for(int h =0 ; h <states.Count ; h++)
        //                {
        //                    if(!states[h].isFinish || !states[h].isFirst)
        //                    {
        //                        for(int i2=0 ;i2 <states.Count ; i2++)
        //                        {
        //                            for(int j2=0 ; j2<states.Count ; j2++)
        //                           {
        //                                if(i2 == h | j2 == h)
        //                                    L[i2][j2] = "remove" ;
        //                           }
        //                        }        
        //                    }
        //                    
        //                }
        //
        //                for(int a =0 ;a< states.Count; a++)
        //                {
        //                    for(int b =0 ; b <states.Count ; b++)
        //                    {
        //                        Console.Write($"L[a][b]       ");
        //                    }
        //                }
        //            return "";
        //        }
        //        private string star(string v)
        //        {   if(v == "$" |  v == "")
        //                return "" ;
        //            return($"({v})*");
        //        }
        //
        //











        public bool isEqual(HashSet<State> a, HashSet<State> b)
        {
            bool equal = true;
            foreach (var q in a)
            {
                equal = b.Contains(q);
            }
            return equal;
        }
        public bool isInHashSet(string value, HashSet<State> lst)
        {
            foreach (var q in lst)
            {
                if (q.value == value)
                    return true;
            }
            return false;
        }


        public DFA convertodfa(string[] alphabet)
        {
            var setofnodes = this.createEquivalentDFA(alphabet);
            Dictionary<string, State> dfastate = new Dictionary<string, State>();

            foreach (var s in setofnodes.Item1)
            {

                if (!dfastate.ContainsKey(s.value))
                {
                    dfastate.Add(s.value, s);
                }


            }
            foreach (var s in dfastate)
            {
                if (s.Value.isvisited)
                {
                    s.Value.isvisited = false;
                }
            }
            return new DFA(dfastate, setofnodes.Item2, alphabet);

        }
         public void ShowNFa()
         {
             Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();

             Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
             foreach (var s in states)
             {
                 foreach (var x in s.nextState)
                 {
                     graph.AddEdge(s.value, x.Item1, x.Item2.value);
                 }
                 graph.FindNode(s.value).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
                 if (s.isFinish)
                {
                    graph.FindNode(s.value).Attr.Shape = Microsoft.Msagl.Drawing.Shape.DoubleCircle; ;
                }

             }
             System.Windows.Forms.Form form = new System.Windows.Forms.Form();
            //bind the graph to the viewer
           viewer.Graph = graph;
            //associate the viewer with the form
            form.SuspendLayout();
             viewer.Dock = System.Windows.Forms.DockStyle.Fill;
             form.Controls.Add(viewer);
           form.ResumeLayout();
            //show the form
            form.ShowDialog();

         }
    }

    class DFA
    {
        public Dictionary<string, State> states;
        public HashSet<State> finalstates;
        public string[] alphabets;
        public HashSet<State> reachable = new HashSet<State>();
        public Dictionary<HashSet<State>, long> convertP = new Dictionary<HashSet<State>, long>();
        public Dictionary<long, HashSet<State>> convertP2 = new Dictionary<long, HashSet<State>>();
        public Dictionary<table, HashSet<State>> part = new Dictionary<table, HashSet<State>>();


        public DFA(Dictionary<string, State> givenStates, HashSet<State> finishStates, string[] alph)
        {
            this.states = givenStates;
            this.reachbles();
            this.finalstates = finishStates;
            this.alphabets = alph;

        }
        public bool isAcceptByDFA(string s)
        {

            State temp = states.Where(x => x.Value.isFirst).First().Value;
            for (int i = 0; i < s.Length; i++)
            {
                IEnumerable<State> t = temp.nextState
                .Where(tuple => tuple.Item1 == s[i].ToString())
                .Select(tuple => tuple.Item2);
                if (t.Count() == 0)
                    return false;
                temp = t.First();
            }
            return temp.isFinish;
        }

        void reachbles()
        {

            Queue<State> q = new Queue<State>();
            State temp = states.Where(x => x.Value.isFirst).First().Value;
            q.Enqueue(temp);
            while (q.Count != 0)
            {
                State curr = q.Dequeue();
                curr.isvisited = true;
                this.reachable.Add(curr);
                foreach (var s in curr.nextState)
                    if (!s.Item2.isvisited)
                        q.Enqueue(s.Item2);
            }
        }


        public DFA minimization()
        {
            var count = states.Count;
            var alphcount = alphabets.Length;
            this.reachbles();
            long[][] hashset = new long[count][];
            HashSet<State> rnonfinal = new HashSet<State>();
            var final = finalstates.Intersect(reachable).ToHashSet();
            rnonfinal = this.reachable;
            rnonfinal.ExceptWith(final);
            HashSet<HashSet<State>> current = new HashSet<HashSet<State>>();
            current.Add(final);
            current.Add(rnonfinal);
            HashSet<HashSet<State>> p = new HashSet<HashSet<State>>();
            while (!isEqual(current, p))
            {
                p = new HashSet<HashSet<State>>(current);
                current = new HashSet<HashSet<State>>(p);
                foreach (var s in p)
                {
                    current.UnionWith(split(s, current));
                    current.Remove(s);

                }
            }

            Dictionary<HashSet<State>, string> convert2p = new Dictionary<HashSet<State>, string>();
            Dictionary<string, HashSet<State>> convertP22 = new Dictionary<string, HashSet<State>>();
            Dictionary<string, State> a = new Dictionary<string, State>();
            long c = 0;

            foreach (var pp in p)
            {
                convert2p.Add(pp, $"q{c}");
                convertP22.Add($"q{c}", pp);

                a.Add($"q{c}", new State($"q{c}"));
                c++;

            }

            HashSet<State> finalfinal;
            HashSet<State> finalminimize = new HashSet<State>();
            foreach (var pp in p)
            {
                var list = pp.FirstOrDefault().nextState;
                finalfinal = new HashSet<State>(pp);
                finalfinal.IntersectWith(finalstates);

                var start = states.Where(x => x.Value.isFirst).FirstOrDefault().Value;
                if (pp.Contains(start))
                {
                    a[convert2p[pp]].isFirst = true;
                }

                foreach (var x in list)
                    foreach (var ppp in p)
                        if (ppp.Contains(x.Item2))
                        {
                            a[convert2p[pp]].addTransition(a[convert2p[ppp]], x.Item1);
                            if (finalfinal.Count != 0)
                            {
                                finalminimize.Add(a[convert2p[pp]]);
                                a[convert2p[pp]].isFinish = true;
                            }

                        }

            }

            Console.WriteLine(p.Count);
            Console.WriteLine(current.Count);


            foreach (var item in a)
            {
                // Console.WriteLine($"{item.value} ");


                foreach (var item1 in item.Value.nextState)
                {
                    // Console.WriteLine(item1.Item1);
                    // Console.WriteLine(item1.Item2.value);
                    Console.WriteLine($"finished:({item.Value.isFinish}) :{item.Value.value}={item1.Item1} ==> {item1.Item2.value}");
                    foreach (var x in convertP22[item.Key])
                    {
                        Console.WriteLine(x.value);
                    }
                    Console.WriteLine("-----------------------");
                }

            }
            return new DFA(a, finalminimize, alphabets);

        }

        public bool isEqual(HashSet<HashSet<State>> one, HashSet<HashSet<State>> two)
        {

            if (one.Count == two.Count)
                foreach (var x in two)
                    if (!iscontains(one, x))
                        return false;

            if (one.Count != two.Count)
                return false;



            return true;
        }

        public bool iscontains(HashSet<HashSet<State>> a, HashSet<State> b)
        {
            foreach (var x in a)
            {
                if (b.SetEquals(x))
                {
                    return true;
                }
            }
            return false;
        }
        /// list with tuple of number of groups> 
        public HashSet<HashSet<State>> split(HashSet<State> s, HashSet<HashSet<State>> p)
        {

            convertP.Clear();
            convertP2.Clear();
            part.Clear();

            long count = 0;
            List<table> tables = new List<table>();

            foreach (var cp in p)
            {
                convertP.Add(cp, count);
                convertP2.Add(count, cp);
                count++;
            }
            foreach (var st in s)
            {
                table ex = new table(st);
                var nextStatee = st.nextState.OrderBy(a => a.Item1).Select(x => x.Item2);
                string u = "";
                foreach (var ns in nextStatee)
                    foreach (var partition in p)
                        if (partition.Contains(ns))
                        {
                            if (u == "")
                                u = $"{convertP[partition]}";

                            else
                                u = u + "," + convertP[partition];

                        }

                ex.value = u;
                tables.Add(ex);
            }

            foreach (var x in tables)
            {
                if (!part.ContainsKey(x))
                {
                    HashSet<State> ex = new HashSet<State>();
                    ex.Add(x.s);
                    part.Add(x, ex);
                }
                else
                    part[x].Add(x.s);

            }
            HashSet<HashSet<State>> newpartition = new HashSet<HashSet<State>>();
            foreach (var h in part)
                newpartition.Add(h.Value);



            return newpartition;
        }
         public void ShowDFa()
         {
             Microsoft.Msagl.GraphViewerGdi.GViewer viewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();

             Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
             foreach (var s in states)
             {
                 foreach (var x in s.Value.nextState)
                 {
                     graph.AddEdge(s.Value.value, x.Item1, x.Item2.value);
                 }
                 graph.FindNode(s.Value.value).Attr.FillColor = Microsoft.Msagl.Drawing.Color.Magenta;
                 if (s.Value.isFinish)
                 {
                    graph.FindNode(s.Value.value).Attr.Shape = Microsoft.Msagl.Drawing.Shape.DoubleCircle; ;
                 }

             }
             System.Windows.Forms.Form form = new System.Windows.Forms.Form();
             //bind the graph to the viewer
             viewer.Graph = graph;
             //associate the viewer with the form
            form.SuspendLayout();
            viewer.Dock = System.Windows.Forms.DockStyle.Fill;
             form.Controls.Add(viewer);
            form.ResumeLayout();
            //show the form
             form.ShowDialog();

         }
    }
    class trans
    {
        public State first;
        public State second;
        public string alphabet;
        public trans(State f, State s, string transition)
        {
            this.first = f;
            this.second = s;
            this.alphabet = transition;

        }
    }
    class table
    {
        public State s;
        public string value { get; set; }
        public table(State s)
        {
            this.s = s;
            this.value = "";
        }
        public override bool Equals(object obj)
        {
            if (obj is table)
                return this.value == (obj as table).value;
            else
                return false;
        }
        public override int GetHashCode() => (int)Convert.ToDouble(this.value);






    }
}