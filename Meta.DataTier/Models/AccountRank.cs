﻿using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class AccountRank
{
    public Guid? Id { get; set; }

    public Guid? AccountId { get; set; }

    public Guid RankId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Rank Rank { get; set; } = null!;
}
