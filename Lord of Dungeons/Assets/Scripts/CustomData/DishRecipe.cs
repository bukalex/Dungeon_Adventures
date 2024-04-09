using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Recipes
{
    [Serializable]
    public class DishRecipe
    {
        public Item dish;
        public Item failedDish;
        public string dishName;
        public List<RequiredIngridients> ingridients = new List<RequiredIngridients>();
    }
    [Serializable]
    public struct RequiredIngridients
    {
        public Item ingridient;
        public int ingridientAmount;
    }
}
