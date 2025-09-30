namespace SMS.Models
{
    public class ExamMarkDistribution
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public int MarkDistributionId { get; set; }
        public MarkDistribution MarkDistribution { get; set; }
    }
}