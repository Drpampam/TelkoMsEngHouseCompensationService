namespace Application.Features.Validations;

public static class GuidCustomValidations
{
    public static bool CheckId(this object id, out Guid output)
    {
        var strId = id.ToString();
        if (Guid.TryParse(strId, out var result))
        {
            output = result;
            return true;
        }

        output = result;
        return false;
    }
}