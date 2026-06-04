
namespace PartnerService.Domain.Shared.ValueObjects;

public class CnpjVO
{
    public string Value { get; private set; }

    public CnpjVO(string value)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("CNPJ is required.");

        if(!Validate(value))
            throw new ArgumentException("Invalid CNPJ.");

        Value = value;
    }

    public static bool Validate(string cnpj)
    {
        if(cnpj.Length != 14)
            return false;

        if(cnpj.All(c => c == cnpj[0]))
            return false;

        return ValidateDigits(cnpj);
    }

    /// <summary cref="https://pt.stackoverflow.com/questions/187106/valida%C3%A7%C3%A3o-cpf-e-cnpj">
    /// Validates the digits of the CNPJ using the standard algorithm.
    /// </summary>
    private static bool ValidateDigits(string cnpj)
    {
        int[] multiplicator1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplicator2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = cnpj.Substring(0, 12);
        int sum = multiplicator1.Select((t, i) => t * (tempCnpj[i] - '0')).Sum();
        int rest = sum % 11;
        char digit1 = rest < 2 ? '0' : (char)((11 - rest) + '0');

        tempCnpj += digit1;
        sum = multiplicator2.Select((t, i) => t * (tempCnpj[i] - '0')).Sum();
        rest = sum % 11;
        char digit2 = rest < 2 ? '0' : (char)((11 - rest) + '0');

        return cnpj.EndsWith($"{digit1}{digit2}");
    }
}