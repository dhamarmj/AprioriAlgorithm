using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apriori
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = ""; //Ruta del dataset
            int soporte = 0; //Soporte minimo
            float confianza = 0; //Confianza Minima
            while (file == "")
            {
                Console.Write("Introduce la Ruta del txt: ");
                file = Console.ReadLine(); //Tomo la ruta
            }
            while (soporte == 0)
            {
                Console.Write("Introduce el soporte mínimo: ");
                int.TryParse(Console.ReadLine(), out soporte);  //Tomo el Soporte Min
            }
            while (confianza == 0)
            {
                Console.Write("Introduce la confiaza mínimo: ");
                float.TryParse(Console.ReadLine(), out confianza); //Tomo la Confianza min
                if (confianza > 1 || confianza < 0)
                {
                    Console.Write("Valor de confianza invalido");
                    confianza = 0;
                }

            }
            Console.WriteLine("Ruta:" + file + " soporte: " + soporte + " confianza: " + confianza);

            string[] ordenes = readFile(file); //Leo el archivo
            List<ItemSet> result = SetUnItem(ordenes, soporte); //Genero las reglas de 1 item y sus soportes
            List<ItemSet> val1 = APriori(ordenes, result, soporte); //Genero todos los itemsets y sus soportes
            List<ItemSet> NoSpaceList = RemoveSpaceItemset(val1); //Quito los espacios para no influenciar al generar 
            List<Regla> R = GenerateRules(NoSpaceList); //Generar las reglas 
            List<Regla> Result = CalculateConfianzaList(NoSpaceList, R, confianza); //Calcular la confianza para cada regla
            Console.WriteLine("Para este Dataset Las reglas importantes son: ....");
            foreach (Regla item in Result)
            {
                Console.WriteLine("Ant: " + item.Antecedente + " Conse: " + item.Consecuente + " Confianza: " + item.Confianza); //Imprime los resultados
            }
            Console.WriteLine("Press anywhere to Exit");
            Console.ReadKey();
        }

        //Quita el espacio en blanco de los nombres del Itemset
        private static List<ItemSet> RemoveSpaceItemset(List<ItemSet> list)
        {
            foreach (ItemSet i in list)
            {
                i.Nombre = i.Nombre.Replace(" ", string.Empty);
            }
            return list;
        }

        private static string[] readFile(string file)
        {
            return File.ReadAllLines(file);
        }

        //Devuelve una lista con los Itemset cuyo soporte paso el minimo
        private static List<ItemSet> classifyResult(List<ItemSet> values, int soporte)
        {
            List<ItemSet> result = new List<ItemSet>();
            foreach (ItemSet i in values)
            {
                if (i.Soporte >= soporte)
                {
                    result.Add(i);
                }
            }
            return result.OrderByDescending(o => o.Soporte).ToList();
        }

        //Agrega los elementos sueltos del archivo a una lista de Itemsets 
        private static List<ItemSet> SetUnItem(string[] ordenes, int soporte)
        {
            List<ItemSet> list = new List<ItemSet>();
            ItemSet A;
            int index = -1;
            foreach (string i in ordenes)
            {
                string[] vals = i.Split(',');
                foreach (string j in vals)
                {
                    A = new ItemSet(j, 1);
                    if (list.Exists(a => a.Nombre == A.Nombre))
                    {
                        index = list.FindIndex(a => a.Nombre == A.Nombre);
                        list[index].Soporte++; //Calculo el soporte de cada Itemset
                    }
                    else
                    {
                        list.Add(A);
                    }
                }

            }

            return classifyResult(list, soporte);
        }
        //Genero todas los itemset y calculo su soporte
        private static List<ItemSet> APriori(string[] ordenes, List<ItemSet> FirstVal, int soporte)
        {
            ItemSet A; int j = 0;
            for (int i = 0; i < FirstVal.Count; i++)
            {
                j = i + 1;
                while (j < FirstVal.Count)
                {
                    A = new ItemSet(FirstVal[i].Nombre + " " + FirstVal[j].Nombre);
                    string[] val = A.Nombre.Split(' ');
                    if (val.Length > 2)
                        A.Nombre = RenameItemset(val); //Elimino elementos repetidos del itemset
                    if (!FirstVal.Exists(a => a.Nombre == A.Nombre)) //No existe
                        if (!CheckPermutationList(FirstVal, A.Nombre)) //No hay otra permutacion de este itemset (ABC == BCA == CBA...etc)
                            FirstVal.Add(A); //Agregalo
                    j++;
                }
            }
            countSoporte(ordenes, FirstVal); //Calcula el soporte
            return classifyResult(FirstVal, soporte); //Devuelve los que tienen soporte mayor que el minimo
        }
        //Elimino elementos repetidos del itemset ej AAB = AB 
        private static string RenameItemset(string[] A)
        {
            string nombre = "";
            var val = A.Distinct().ToList();
            foreach (var i in val)
            {
                nombre += i + " ";
            }
            return nombre.Trim();

        }
        //Calcular Soporte para una lista
        private static void countSoporte(string[] ordenes, List<ItemSet> list)
        {

            foreach (ItemSet item in list)
            {
                if (item.Soporte == 0) //Si no se ha calculado
                {
                    foreach (string i in ordenes)
                    {
                        if (countSop(item.Nombre.Split(' '), i)) //Es el que busco? 
                            item.Soporte++; //Suma
                    }
                }
            }

        }
        //Soporte de itemsets con +1 valor ej ABC, ABCD
        private static bool countSop(string[] nombres, string ordenes)
        {
            int count = 0;
            foreach (var item in nombres)
            {
                if (ordenes.Contains(item)) //Esta 1 de los itemes de ese itemset?
                    count++;
            }
            if (count == nombres.Length) //Encontre todos los elementos del itemset?
                return true; //Suma 1

            else
                return false;//No fue encontrado en esta transaccion
        }

        //Codigo COPIADO y modificado de -> https://www.geeksforgeeks.org/check-if-two-strings-are-permutation-of-each-other/
        //Nos ayuda a saber si un string es permutación de otro
        static bool arePermutation(string str1, string str2)
        {
            if (str1.Length != str2.Length)
                return false;
            char[] ch1 = str1.ToCharArray();
            char[] ch2 = str2.ToCharArray();

            Array.Sort(ch1);
            Array.Sort(ch2);

            for (int i = 0; i < str1.Length; i++)
                if (ch1[i] != ch2[i])
                    return false;

            return true;
        }
        //Reviso si el valor a entrar (newVal) ya esta en la lista
        private static bool CheckPermutationList(List<ItemSet> list, string newVal)
        {
            int i = 0;
            while (i < list.Count)
            {
                if (arePermutation(list[i].Nombre, newVal))
                    return true;
                i++;
            }
            return false;
        }
        //codigo COPIADO y modificado de -> https://github.com/Omar-Salem/Apriori-Algorithm/blob/master/AprioriAlgorithm/Implementation/Apriori.cs
        //Genera todas las reglas posibles desde la lista de itemsets validos
        private static List<Regla> GenerateRules(List<ItemSet> allFrequentItems)
        {
            var rulesList = new List<Regla>();

            foreach (var item in allFrequentItems)
            {
                if (item.Nombre.Length > 1)
                {
                    IEnumerable<string> subsetsList = GenerateSubsets(item.Nombre);

                    foreach (var subset in subsetsList)
                    {
                        string remaining = GetRemaining(subset, item.Nombre);
                        Regla rule = new Regla(subset, remaining);

                        if (!rulesList.Contains(rule))
                        {
                            rulesList.Add(rule);
                        }
                    }
                }
            }

            return rulesList;
        }
        //codigo COPIADO y modificado de -> https://github.com/Omar-Salem/Apriori-Algorithm/blob/master/AprioriAlgorithm/Implementation/Apriori.cs
        private static IEnumerable<string> GenerateSubsets(string item)
        {
            IEnumerable<string> allSubsets = new string[] { };
            int subsetLength = item.Length / 2;

            for (int i = 1; i <= subsetLength; i++)
            {
                IList<string> subsets = new List<string>();
                GenerateSubsetsRecursive(item, i, new char[item.Length], subsets);
                allSubsets = allSubsets.Concat(subsets);
            }

            return allSubsets;
        }
        //codigo COPIADO y modificado de -> https://github.com/Omar-Salem/Apriori-Algorithm/blob/master/AprioriAlgorithm/Implementation/Apriori.cs
        private static void GenerateSubsetsRecursive(string item, int subsetLength, char[] temp, IList<string> subsets, int q = 0, int r = 0)
        {
            if (q == subsetLength)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < subsetLength; i++)
                {
                    sb.Append(temp[i]);
                }

                subsets.Add(sb.ToString());
            }

            else
            {
                for (int i = r; i < item.Length; i++)
                {
                    temp[q] = item[i];
                    GenerateSubsetsRecursive(item, subsetLength, temp, subsets, q + 1, i + 1);
                }
            }
        }

        //codigo COPIADO y modificado de -> https://github.com/Omar-Salem/Apriori-Algorithm/blob/master/AprioriAlgorithm/Implementation/Apriori.cs
        private static string GetRemaining(string child, string parent)
        {
            for (int i = 0; i < child.Length; i++)
            {
                int index = parent.IndexOf(child[i]);
                parent = parent.Remove(index, 1);
            }

            return parent;
        }

        //Calculo la confianza de todas las reglas
        private static List<Regla> CalculateConfianzaList(List<ItemSet> Itemsetlist, List<Regla> list, float confianza)
        {
            foreach (Regla item in list)
            {
                item.Confianza = CalculateConfianzaSingle(Itemsetlist, item.Antecedente + item.Consecuente, item.Antecedente);
            }
            return ClassifyConfianza(list, confianza);
        }
        //Calcula la confianza de 1 sola regla
        private static float CalculateConfianzaSingle(List<ItemSet> list, string regla, string ante)
        {
            int i = 0, valR = -1, valA = -1;
            while (i < list.Count && (valR == -1 || valA == -1))
            {
                if (arePermutation(list[i].Nombre, regla)) //Busco el soporte de la regla completa (teniendo en cuenta que el orden no influye)
                    valR = list[i].Soporte;
                else if (list[i].Nombre == ante) //Busco el soporte del antecedente
                    valA = list[i].Soporte;

                i++;
            }
            float conf = (float)valR / (float)valA; //Calculo
            return conf;

        }

        //Valida las reglas cuya confianza sea mayor que el minimo
        private static List<Regla> ClassifyConfianza(List<Regla> list, float confianza)
        {
            List<Regla> result = new List<Regla>();
            foreach (Regla item in list)
            {
                if (item.Confianza >= confianza)
                    result.Add(item);
            }
            //Ordena el resultado por el numero de elementos en la regla y luego por la confianza 
            return result.OrderByDescending(item => (item.Antecedente + item.Consecuente).Length)
                         .ThenBy(item => item.Confianza).ToList();
        }

    }
}


