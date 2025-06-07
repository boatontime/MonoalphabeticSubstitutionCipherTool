using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Suggest
{
    public static string GetSuggest(string cipherText, Guess[] guesskey, NGramFreq[][] nGramFreqRefs, int auto)
    {
        // 0. ��ʼ������͸��ʾ���
        StringBuilder report = new StringBuilder();
        double[,] probMatrix = new double[27, 27];
        cipherText = cipherText.ToUpper();

        // 1. ��ʼ�����ʾ��󣨿���guesskeyԼ����
        InitializeProbabilityMatrix(probMatrix, guesskey, report);

        // �������ģ�ȥ���ո�
        string cleanText = new string(cipherText.Where(c => char.IsLetter(c)).ToArray());
        report.AppendLine($"[������ʼ]");
        report.AppendLine("----------------------------------");

        // 2. 1��ĸƵ�ʷ���
        report.AppendLine("\n=== 1��ĸƵ�ʷ��� ===");
        AnalyzeNGrams(cleanText, 1, probMatrix, nGramFreqRefs[1], guesskey, report);

        // 3. 2��ĸƵ�ʷ���
        report.AppendLine("\n=== 2��ĸƵ�ʷ��� ===");
        AnalyzeNGrams(cleanText, 2, probMatrix, nGramFreqRefs[2], guesskey, report);

        // 4. 3��ĸƵ�ʷ���
        report.AppendLine("\n=== 3��ĸƵ�ʷ��� ===");
        AnalyzeNGrams(cleanText, 3, probMatrix, nGramFreqRefs[3], guesskey, report);

        // 5. 4��ĸƵ�ʷ���
        report.AppendLine("\n=== 4��ĸƵ�ʷ��� ===");
        AnalyzeNGrams(cleanText, 4, probMatrix, nGramFreqRefs[4], guesskey, report);

        // 6. ��ĸ���ӷ���
        report.AppendLine("\n=== ��ĸ���ӷ��� ===");
        AnalyzeLetterConnections(cleanText, probMatrix, guesskey, report);
        NormalizeProbabilityMatrix(probMatrix, guesskey);

        // �����������
        report.AppendLine("\n[���ʾ���������]");
        report.AppendLine("----------------------------------");
        GenerateProbabilityReport(probMatrix, guesskey, report, auto);

        return report.ToString();
    }

    // ========== ���ĸ������� ==========

    private static void InitializeProbabilityMatrix(double[,] matrix, Guess[] guesskey, StringBuilder report)
    {
        int initializedCount = 0;
        int constrainedCount = 0;
        int sureCount = 0;

        for (int p = 1; p <= 26; p++)
        {
            for (int c = 1; c <= 26; c++)
            {
                // Ӧ��guesskeyԼ��
                if (guesskey[p].guess[c])
                {
                    matrix[p, c] = 0; // ������ӳ��
                    constrainedCount++;
                }
                else if (guesskey[p].sure && guesskey[p].sureChar == c)
                {
                    matrix[p, c] = 1.0; // ��ȷ��ӳ��
                    sureCount++;
                }
                else
                {
                    matrix[p, c] = 0.5; // ��ʼ����
                    initializedCount++;
                }
            }
        }

        report.AppendLine($"���ʾ����ʼ��");
        report.AppendLine($"- ��ȷ��ӳ��: {sureCount}");
        report.AppendLine($"- ��ʼ���ʵ�: {initializedCount}");
        report.AppendLine($"- �ų�ӳ���: {constrainedCount}");
    }

    private static void AnalyzeNGrams(string text, int n, double[,] matrix,
                                     NGramFreq[] freqRefs, Guess[] guesskey,
                                     StringBuilder report)
    {
        if (freqRefs == null || freqRefs.Length == 0)
        {
            report.AppendLine($"δ�ṩ{n}��ĸ�ο�Ƶ��");
            return;
        }

        // 1. ��ȡ����n-gramƵ��
        var cipherFreq = GetNGrams(text, n);
        if (cipherFreq.Count == 0)
        {
            report.AppendLine($"����δ�ҵ�{n}��ĸ���");
            return;
        }

        // 2. �ҳ����10��δ����ȫ����ĸ�Ƶn��ĸ���
        var topCandidates = new List<KeyValuePair<string, int>>();
        foreach (var pair in cipherFreq.OrderByDescending(p => p.Value))
        {
            string ngram = pair.Key;

            // ����Ƿ��ѱ���ȫ����
            bool fullyDecoded = true;
            foreach (char c in ngram)
            {
                int cipherIndex = c - 'A' + 1;
                if (GlobalVariable.instance.key[cipherIndex] == 0)
                {
                    fullyDecoded = false;
                    break;
                }
            }

            // ֻѡ��δ��ȫ��������
            if (!fullyDecoded)
            {
                topCandidates.Add(pair);
            }
        }

        if (topCandidates.Count == 0)
        {
            report.AppendLine($"����{n}��ĸ����ѱ���ȫ����");
            return;
        }

        report.AppendLine($"���� {topCandidates.Count} ��δ��ȫ����ĸ�Ƶ{n}��ĸ���");
        int sum = 0;
        // 3. ��ÿ����ѡn��ĸ��Ͻ��з���
        foreach (var candidate in topCandidates)
        {
            string cipherNGram = candidate.Key;
            int cipherCount = candidate.Value;
            double cipherProb = cipherCount / (double)cipherFreq.Values.Sum();


            // 4. �ڲο������ҳ����10������ƥ���n��ĸ���
            var possibleMatches = new List<NGramFreq>();
            foreach (var refItem in freqRefs.OrderByDescending(f => f.frequency))
            {
                string refNGram = refItem.ngram;

                // ����Ƿ���guesskey��ͻ
                bool conflict = false;
                for (int i = 0; i < n; i++)
                {
                    char refChar = refNGram[i];
                    char cipherChar = cipherNGram[i];

                    int plainIndex = refChar - 'A' + 1;
                    int cipherIndex = cipherChar - 'A' + 1;

                    // ��ͻ������ӳ�䱻�ų�����ȷ������ƥ��
                    if (guesskey[plainIndex].guess[cipherIndex] ||
                        (guesskey[plainIndex].sure && guesskey[plainIndex].sureChar != cipherIndex))
                    {
                        conflict = true;
                        break;
                    }
                }

                if (!conflict)
                {
                    possibleMatches.Add(refItem);
                    if (possibleMatches.Count >= 10) break;
                }
            }

            if (possibleMatches.Count == 0)
            {
                continue;
            }

            report.AppendLine($"\n- ���� '{cipherNGram}' (Ƶ��: {cipherProb:P2})");
            report.Append($"�ҵ� {possibleMatches.Count} ���޳�ͻ�ο����:");
            sum++;
            // 5. ��ÿ��ƥ��Ĳο���ϣ���߶�Ӧλ�õĸ���
            foreach (var match in possibleMatches)
            {
                string refNGram = match.ngram;
                double refFreq = match.frequency;
                double similarity = 1.0 - Math.Abs(cipherProb - refFreq);
                int suresum = 0;

                report.Append($"{refNGram} ");
                for (int i = 0; i < n; i++)
                {
                    char cipherChar = cipherNGram[i];
                    int cipherIndex = cipherChar - 'A' + 1;
                    if (GlobalVariable.instance.key[cipherIndex] != 0)
                    {
                        suresum++;
                    }
                }
                // ���ÿ��λ�õ�ӳ�����
                for (int i = 0; i < n; i++)
                {
                    char refChar = refNGram[i];
                    char cipherChar = cipherNGram[i];

                    int plainIndex = refChar - 'A' + 1;
                    int cipherIndex = cipherChar - 'A' + 1;

                    // ������ȷ����ӳ��
                    if (guesskey[plainIndex].sure) continue;

                    // �������ƶ���߸��ʣ����ƶ�Խ�ߣ�����Խ��
                    double boost = 1.0 + (similarity * 0.5) + (1.0 / possibleMatches.Count) * 0.1 + suresum * 0.1;
                    matrix[plainIndex, cipherIndex] *= boost;

                }
            }
            if (sum > 10)
                break;
        }
    }

    private static Dictionary<string, int> GetNGrams(string text, int n)
    {
        text = text.ToUpper().Where(char.IsLetter).Aggregate("", (current, c) => current + c);
        var dict = new Dictionary<string, int>();

        for (int i = 0; i <= text.Length - n; i++)
        {
            string ngram = text.Substring(i, n);
            if (dict.ContainsKey(ngram)) dict[ngram]++;
            else dict[ngram] = 1;
        }
        return dict;
    }

    private static void NormalizeProbabilityMatrix(double[,] matrix, Guess[] guesskey)
    {
        for (int p = 1; p <= 26; p++)
        {
            // ������ȷ������
            if (guesskey[p].sure) continue;

            double sum = 0;
            for (int c = 1; c <= 26; c++)
            {
                // �������ų���ӳ��
                if (guesskey[p].guess[c]) continue;
                sum += matrix[p, c];
            }

            if (sum > double.Epsilon)
            {
                for (int c = 1; c <= 26; c++)
                {
                    if (guesskey[p].guess[c]) continue;
                    matrix[p, c] /= sum;
                }
            }
        }
    }

    private static void GenerateProbabilityReport(double[,] matrix, Guess[] guesskey, StringBuilder report, int auto)
    {
        // �ռ����и��ʵ㣨�ų���ȷ���Ͳ����ܵĵ㣩
        var allProbabilities = new List<(int plain, int cipher, double prob)>();

        for (int p = 1; p <= 26; p++)
        {
            // ������ȷ����������ĸ
            if (guesskey[p].sure) continue;

            for (int c = 1; c <= 26; c++)
            {
                // �������ų���ӳ��
                if (guesskey[p].guess[c]) continue;

                allProbabilities.Add((p, c, matrix[p, c]));
            }
        }

        // ����������
        var sortedProbs = allProbabilities.OrderByDescending(x => x.prob).ToList();

        if (auto == 1)
        {
            report.Clear();
            report.Append($"guess 0 {(char)(sortedProbs[0].plain + 64)} {(char)(sortedProbs[0].cipher + 64)}");
            return;
        }
        // �������ܵ�5������
        report.AppendLine("[����ܵĲ���]");
        for (int i = 0; i < Math.Min(5, sortedProbs.Count); i++)
        {
            var item = sortedProbs[i];
            report.AppendLine($"[{(char)(item.plain+64)}->{(char)(item.cipher + 64)}] ����:{item.prob:P4}");
        }

        // �������ܵ�5������
        report.AppendLine("\n[����ܵĲ���]");
        for (int i = Math.Max(0, sortedProbs.Count - 5); i < sortedProbs.Count; i++)
        {
            var item = sortedProbs[i];
            report.AppendLine($"[{(char)(item.plain + 64)}->{(char)(item.cipher + 64)}] ����:{item.prob:P4}");
        }
    }

    private static void AnalyzeLetterConnections(string cipherText, double[,] matrix1,
                                           Guess[] guesskey, StringBuilder report)
    {
        double[,] matrix = new double[27, 27];
        // 1. ���ʿ�ͷģʽ����
        string[] startPatterns = { "THE", "STR", "PL", "WH", "HI" };
        AnalyzeWordStartPatterns(cipherText, startPatterns, matrix, guesskey, report);

        // 2. ���ʽ�βģʽ����
        string[] endPatterns = { "ING", "ED", "ION" };
        AnalyzeWordEndPatterns(cipherText, endPatterns, matrix, guesskey, report);

        // 3. ����λ��ģʽ����
        string[] anyPositionPatterns = { "QU", "TH", "HE", "IN", "ER", "RES",
                                    "CA", "CO", "BA", "DA", "ET", "AT",
                                    "ON", "IT", "LL", "EE", "SS", "OO", "TT" };
        AnalyzeAnyPositionPatterns(cipherText, anyPositionPatterns, matrix, guesskey, report);

    }

    private static void AnalyzeWordStartPatterns(string text, string[] patterns,
                                                double[,] matrix, Guess[] guesskey,
                                                StringBuilder report)
    {
        report.AppendLine("\n[���ʿ�ͷ����]");
        string[] words = text.Split(' ');

        foreach (string word in words)
        {
            if (word.Length < 3) continue; // ����3��ĸ����

            foreach (string pattern in patterns)
            {
                if (word.Length < pattern.Length) continue;

                string start = word.Substring(0, pattern.Length);

                for (int i = 0; i < pattern.Length; i++)
                {
                    char cipherChar = start[i];
                    char plainChar = pattern[i];

                    int plainIndex = plainChar - 'A' + 1;
                    int cipherIndex = cipherChar - 'A' + 1;

                    // �������ų�����ȷ����ӳ��
                    if (guesskey[plainIndex].guess[cipherIndex]) continue;
                    if (guesskey[plainIndex].sure) continue;

                    // ����λ�ü�Ȩ����ǿ20%��
                    double boost = 1.2;
                    matrix[plainIndex, cipherIndex] *= boost;

                }
            }
        }
    }

    private static void AnalyzeWordEndPatterns(string text, string[] patterns,
                                              double[,] matrix, Guess[] guesskey,
                                              StringBuilder report)
    {
        report.AppendLine("\n[���ʽ�β����]");
        string[] words = text.Split(' ');

        foreach (string word in words)
        {
            if (word.Length < 3) continue; // ����3��ĸ����

            foreach (string pattern in patterns)
            {
                if (word.Length < pattern.Length) continue;

                string end = word.Substring(word.Length - pattern.Length);

                for (int i = 0; i < pattern.Length; i++)
                {
                    char cipherChar = end[i];
                    char plainChar = pattern[i];

                    int plainIndex = plainChar - 'A' + 1;
                    int cipherIndex = cipherChar - 'A' + 1;

                    // �������ų�����ȷ����ӳ��
                    if (guesskey[plainIndex].guess[cipherIndex]) continue;
                    if (guesskey[plainIndex].sure) continue;

                    // ��βλ�ü�Ȩ����ǿ15%��
                    double boost = 1.15;
                    matrix[plainIndex, cipherIndex] *= boost;

                }
            }
        }
    }

    private static void AnalyzeAnyPositionPatterns(string text, string[] patterns,
                                                  double[,] matrix, Guess[] guesskey,
                                                  StringBuilder report)
    {
        report.AppendLine("\n[����λ��ģʽ����]");

        foreach (string pattern in patterns)
        {
            for (int i = 0; i <= text.Length - pattern.Length; i++)
            {
                string segment = text.Substring(i, pattern.Length);

                // ������������ĸ�ַ��Ķ�
                if (segment.Any(c => !char.IsLetter(c))) continue;


                for (int j = 0; j < pattern.Length; j++)
                {
                    char cipherChar = segment[j];
                    char plainChar = pattern[j];

                    int plainIndex = plainChar - 'A' + 1;
                    int cipherIndex = cipherChar - 'A' + 1;

                    // �������ų�����ȷ����ӳ��
                    if (guesskey[plainIndex].guess[cipherIndex]) continue;
                    if (guesskey[plainIndex].sure) continue;

                    // ������ǿ����
                    double boost = 1.1;

                    // ˫д��ĸ������ǿ��LL/EE/SS�ȣ�
                    if (pattern.Length == 2 && pattern[0] == pattern[1])
                    {
                        boost = 1.3; // ������ǿ
                    }

                    matrix[plainIndex, cipherIndex] *= boost;
                }
            }
        }
    }
}