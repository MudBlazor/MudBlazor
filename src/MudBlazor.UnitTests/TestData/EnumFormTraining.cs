// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.DataAnnotations;

namespace MudBlazor.UnitTests.TestData;

/// <summary>
/// This is test enum for using in datagrid filters
/// </summary>
public enum EnumFormTraining
{
    [Display(Name = "Бюджетная оплата", Description = "Постующие на бесплатной основе")]
    Free,
    [Display(Name = "Внебюджетная оплата", Description = "Постующие на платной основе")]
    Paid
}
