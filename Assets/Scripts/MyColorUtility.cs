using UnityEngine;

//�J���[�R�[�h�������Color�N���X�ɕϊ�����X�N���v�g
//https://baba-s.hatenablog.com/entry/2015/12/31/100000
public static class MyColorUtility
{
    /// <summary>
    /// �w�肳�ꂽ������� Color �^�ɕϊ��ł���ꍇ true ��Ԃ��܂�
    /// </summary>
    public static bool IsColor(string htmlString)
    {
        Color color;
        return ColorUtility.TryParseHtmlString(htmlString, out color);
    }

    /// <summary>
    /// �w�肳�ꂽ������� Color �^�ɕϊ����܂�
    /// </summary>
    public static Color ToColor(string htmlString)
    {
        Color color;
        ColorUtility.TryParseHtmlString(htmlString, out color);
        return color;
    }

    /// <summary>
    /// <para>�w�肳�ꂽ������� Color �^�ɕϊ����܂�</para>
    /// <para>�ϊ��ł��Ȃ������ꍇ�f�t�H���g�l��Ԃ��܂�</para>
    /// </summary>
    public static Color ToColorOrDefault(string htmlString, Color defaultValue = default(Color))
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(htmlString, out color))
        {
            return color;
        }
        return defaultValue;
    }

    /// <summary>
    /// <para>�w�肳�ꂽ������� Color �^�ɕϊ����܂�</para>
    /// <para>�ϊ��ł��Ȃ������ꍇ null ��Ԃ��܂�</para>
    /// </summary>
    public static Color? ToColorOrNull(string htmlString)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(htmlString, out color))
        {
            return color;
        }
        return null;
    }
}