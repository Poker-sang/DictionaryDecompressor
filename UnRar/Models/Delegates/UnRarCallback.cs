using UnRar.Models.Enums;

namespace UnRar.Models.Delegates;

public delegate CallbackMode UnRarCallback(CallbackMessages msg, long userData, long p1, long p2);
