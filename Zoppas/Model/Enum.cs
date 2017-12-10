namespace Zoppas.Model
{
    /// <summary>
    /// 咖啡壶型号
    /// </summary>
    public enum TYPE_COFFEE
    {
        T_7319 = 1,
        T_7320,
    }

    /// <summary>
    /// 检测项类型
    /// </summary>
    public enum TYPE_ITEM
    {
        NORMAL,
        DIFF,
        MULTI,
        BETWEEN,
    }

    /// <summary>
    /// 自动模式分为上壶，工作，清壶三种情况
    /// </summary>
    public enum WORK_MODE
    {
        PUSH,
        WORK,
        POP,
    }

    /// <summary>
    /// 系统工作模式分为校准，自动两种情况
    /// </summary>
    public enum SYSTEM_MODE
    {
        NONE,
        CALIBRATION,
        AUTO,
    }

    /// <summary>
    /// 小电机IO INPUT
    /// </summary>
    public enum IO_INPUT_MOTOR
    {
        OP10 = 0,
        OP20,
        OP30,
        OP40,
        OP50,
        OP60,
        OP70,
        OP80,
    }

    /// <summary>
    /// 小电机IO OUTPUT
    /// </summary>
    public enum IO_OUTPUT_MOTOR
    {
        OP10 = 0,
        OP20,
        OP30,
        OP40,
        OP50,
        OP60,
        OP70,
        OP80,
    }
}
