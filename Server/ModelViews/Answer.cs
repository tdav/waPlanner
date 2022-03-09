namespace waPlanner.ModelViews
{
    public record Answer<T>(bool IsSuccess, string Message, T Data);

    public record AnswerBasic(bool IsSuccess, string Message);

}
