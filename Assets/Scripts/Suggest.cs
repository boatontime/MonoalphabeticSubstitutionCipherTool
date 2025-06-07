using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Suggest
{
    public static string GetSuggest(string cipherText, Guess[] guesskey, NGramFreq[][] nGramFreqRefs, int auto)
    {
        // 0. 初始化报告和概率矩阵
        StringBuilder report = new StringBuilder();
        double[,] probMatrix = new double[27, 27];
        cipherText = cipherText.ToUpper();

        // 1. 初始化概率矩阵（考虑guesskey约束）
        InitializeProbabilityMatrix(probMatrix, guesskey, report);

        // 处理密文（去除空格）
        string cleanText = new string(cipherText.Where(c => char.IsLetter(c)).ToArray());
        report.AppendLine($"[分析开始]");
        report.AppendLine("----------------------------------");

        // 2. 1字母频率分析
        report.AppendLine("\n=== 1字母频率分析 ===");
        AnalyzeNGrams(cleanText, 1, probMatrix, nGramFreqRefs[1], guesskey, report);

        // 3. 2字母频率分析
        report.AppendLine("\n=== 2字母频率分析 ===");
        AnalyzeNGrams(cleanText, 2, probMatrix, nGramFreqRefs[2], guesskey, report);

        // 4. 3字母频率分析
        report.AppendLine("\n=== 3字母频率分析 ===");
        AnalyzeNGrams(cleanText, 3, probMatrix, nGramFreqRefs[3], guesskey, report);

        // 5. 4字母频率分析
        report.AppendLine("\n=== 4字母频率分析 ===");
        AnalyzeNGrams(cleanText, 4, probMatrix, nGramFreqRefs[4], guesskey, report);

        // 6. 字母连接分析
        report.AppendLine("\n=== 字母连接分析 ===");
        AnalyzeLetterConnections(cleanText, probMatrix, guesskey, report);
        NormalizeProbabilityMatrix(probMatrix, guesskey);

        // 生成最终输出
        report.AppendLine("\n[概率矩阵分析结果]");
        report.AppendLine("----------------------------------");
        GenerateProbabilityReport(probMatrix, guesskey, report, auto);

        return report.ToString();
    }

    // ========== 核心辅助函数 ==========

    private static void InitializeProbabilityMatrix(double[,] matrix, Guess[] guesskey, StringBuilder report)
    {
        int initializedCount = 0;
        int constrainedCount = 0;
        int sureCount = 0;

        for (int p = 1; p <= 26; p++)
        {
            for (int c = 1; c <= 26; c++)
            {
                // 应用guesskey约束
                if (guesskey[p].guess[c])
                {
                    matrix[p, c] = 0; // 不可能映射
                    constrainedCount++;
                }
                else if (guesskey[p].sure && guesskey[p].sureChar == c)
                {
                    matrix[p, c] = 1.0; // 已确定映射
                    sureCount++;
                }
                else
                {
                    matrix[p, c] = 0.5; // 初始概率
                    initializedCount++;
                }
            }
        }

        report.AppendLine($"概率矩阵初始化");
        report.AppendLine($"- 已确定映射: {sureCount}");
        report.AppendLine($"- 初始概率点: {initializedCount}");
        report.AppendLine($"- 排除映射点: {constrainedCount}");
    }

    private static void AnalyzeNGrams(string text, int n, double[,] matrix,
                                     NGramFreq[] freqRefs, Guess[] guesskey,
                                     StringBuilder report)
    {
        if (freqRefs == null || freqRefs.Length == 0)
        {
            report.AppendLine($"未提供{n}字母参考频率");
            return;
        }

        // 1. 获取密文n-gram频率
        var cipherFreq = GetNGrams(text, n);
        if (cipherFreq.Count == 0)
        {
            report.AppendLine($"密文未找到{n}字母组合");
            return;
        }

        // 2. 找出最多10个未被完全解码的高频n字母组合
        var topCandidates = new List<KeyValuePair<string, int>>();
        foreach (var pair in cipherFreq.OrderByDescending(p => p.Value))
        {
            string ngram = pair.Key;

            // 检查是否已被完全解码
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

            // 只选择未完全解码的组合
            if (!fullyDecoded)
            {
                topCandidates.Add(pair);
            }
        }

        if (topCandidates.Count == 0)
        {
            report.AppendLine($"所有{n}字母组合已被完全解码");
            return;
        }

        report.AppendLine($"分析 {topCandidates.Count} 个未完全解码的高频{n}字母组合");
        int sum = 0;
        // 3. 对每个候选n字母组合进行分析
        foreach (var candidate in topCandidates)
        {
            string cipherNGram = candidate.Key;
            int cipherCount = candidate.Value;
            double cipherProb = cipherCount / (double)cipherFreq.Values.Sum();


            // 4. 在参考表中找出最多10个可能匹配的n字母组合
            var possibleMatches = new List<NGramFreq>();
            foreach (var refItem in freqRefs.OrderByDescending(f => f.frequency))
            {
                string refNGram = refItem.ngram;

                // 检查是否与guesskey冲突
                bool conflict = false;
                for (int i = 0; i < n; i++)
                {
                    char refChar = refNGram[i];
                    char cipherChar = cipherNGram[i];

                    int plainIndex = refChar - 'A' + 1;
                    int cipherIndex = cipherChar - 'A' + 1;

                    // 冲突条件：映射被排除或已确定但不匹配
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

            report.AppendLine($"\n- 密文 '{cipherNGram}' (频率: {cipherProb:P2})");
            report.Append($"找到 {possibleMatches.Count} 个无冲突参考组合:");
            sum++;
            // 5. 对每个匹配的参考组合，提高对应位置的概率
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
                // 提高每个位置的映射概率
                for (int i = 0; i < n; i++)
                {
                    char refChar = refNGram[i];
                    char cipherChar = cipherNGram[i];

                    int plainIndex = refChar - 'A' + 1;
                    int cipherIndex = cipherChar - 'A' + 1;

                    // 跳过已确定的映射
                    if (guesskey[plainIndex].sure) continue;

                    // 基于相似度提高概率（相似度越高，提升越大）
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
            // 跳过已确定的行
            if (guesskey[p].sure) continue;

            double sum = 0;
            for (int c = 1; c <= 26; c++)
            {
                // 跳过被排除的映射
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
        // 收集所有概率点（排除已确定和不可能的点）
        var allProbabilities = new List<(int plain, int cipher, double prob)>();

        for (int p = 1; p <= 26; p++)
        {
            // 跳过已确定的明文字母
            if (guesskey[p].sure) continue;

            for (int c = 1; c <= 26; c++)
            {
                // 跳过被排除的映射
                if (guesskey[p].guess[c]) continue;

                allProbabilities.Add((p, c, matrix[p, c]));
            }
        }

        // 按概率排序
        var sortedProbs = allProbabilities.OrderByDescending(x => x.prob).ToList();

        if (auto == 1)
        {
            report.Clear();
            report.Append($"guess 0 {(char)(sortedProbs[0].plain + 64)} {(char)(sortedProbs[0].cipher + 64)}");
            return;
        }
        // 输出最可能的5个猜想
        report.AppendLine("[最可能的猜想]");
        for (int i = 0; i < Math.Min(5, sortedProbs.Count); i++)
        {
            var item = sortedProbs[i];
            report.AppendLine($"[{(char)(item.plain+64)}->{(char)(item.cipher + 64)}] 概率:{item.prob:P4}");
        }

        // 输出最不可能的5个猜想
        report.AppendLine("\n[最不可能的猜想]");
        for (int i = Math.Max(0, sortedProbs.Count - 5); i < sortedProbs.Count; i++)
        {
            var item = sortedProbs[i];
            report.AppendLine($"[{(char)(item.plain + 64)}->{(char)(item.cipher + 64)}] 概率:{item.prob:P4}");
        }
    }

    private static void AnalyzeLetterConnections(string cipherText, double[,] matrix1,
                                           Guess[] guesskey, StringBuilder report)
    {
        double[,] matrix = new double[27, 27];
        // 1. 单词开头模式分析
        string[] startPatterns = { "THE", "STR", "PL", "WH", "HI" };
        AnalyzeWordStartPatterns(cipherText, startPatterns, matrix, guesskey, report);

        // 2. 单词结尾模式分析
        string[] endPatterns = { "ING", "ED", "ION" };
        AnalyzeWordEndPatterns(cipherText, endPatterns, matrix, guesskey, report);

        // 3. 任意位置模式分析
        string[] anyPositionPatterns = { "QU", "TH", "HE", "IN", "ER", "RES",
                                    "CA", "CO", "BA", "DA", "ET", "AT",
                                    "ON", "IT", "LL", "EE", "SS", "OO", "TT" };
        AnalyzeAnyPositionPatterns(cipherText, anyPositionPatterns, matrix, guesskey, report);

    }

    private static void AnalyzeWordStartPatterns(string text, string[] patterns,
                                                double[,] matrix, Guess[] guesskey,
                                                StringBuilder report)
    {
        report.AppendLine("\n[单词开头分析]");
        string[] words = text.Split(' ');

        foreach (string word in words)
        {
            if (word.Length < 3) continue; // 至少3字母单词

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

                    // 跳过被排除或已确定的映射
                    if (guesskey[plainIndex].guess[cipherIndex]) continue;
                    if (guesskey[plainIndex].sure) continue;

                    // 词首位置加权（增强20%）
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
        report.AppendLine("\n[单词结尾分析]");
        string[] words = text.Split(' ');

        foreach (string word in words)
        {
            if (word.Length < 3) continue; // 至少3字母单词

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

                    // 跳过被排除或已确定的映射
                    if (guesskey[plainIndex].guess[cipherIndex]) continue;
                    if (guesskey[plainIndex].sure) continue;

                    // 词尾位置加权（增强15%）
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
        report.AppendLine("\n[任意位置模式分析]");

        foreach (string pattern in patterns)
        {
            for (int i = 0; i <= text.Length - pattern.Length; i++)
            {
                string segment = text.Substring(i, pattern.Length);

                // 跳过包含非字母字符的段
                if (segment.Any(c => !char.IsLetter(c))) continue;


                for (int j = 0; j < pattern.Length; j++)
                {
                    char cipherChar = segment[j];
                    char plainChar = pattern[j];

                    int plainIndex = plainChar - 'A' + 1;
                    int cipherIndex = cipherChar - 'A' + 1;

                    // 跳过被排除或已确定的映射
                    if (guesskey[plainIndex].guess[cipherIndex]) continue;
                    if (guesskey[plainIndex].sure) continue;

                    // 基础增强因子
                    double boost = 1.1;

                    // 双写字母特殊增强（LL/EE/SS等）
                    if (pattern.Length == 2 && pattern[0] == pattern[1])
                    {
                        boost = 1.3; // 额外增强
                    }

                    matrix[plainIndex, cipherIndex] *= boost;
                }
            }
        }
    }
}