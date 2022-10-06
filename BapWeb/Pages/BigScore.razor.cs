﻿using Microsoft.AspNetCore.Components;

namespace BapWeb.Pages
{
    public partial class BigScore
    {
        [Parameter]
        public string ScoreText { get; set; } = "";
        [Parameter]
        public string ScoreDescriptionLineOne { get; set; } = "";
        [Parameter]
        public string? ScoreDescriptionLineTwo { get; set; } = "";
    }
}
