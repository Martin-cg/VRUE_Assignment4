﻿using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Recipe {
    public string RecipeName;
    public List<Recipe.Ingredient> Ingredients;

    public void Serialize(PhotonStream stream) {
        if (stream.IsWriting) {
            stream.SendNext(RecipeName);
            int IngredientCount = Ingredients.Count;
            stream.SendNext(IngredientCount);
            for (int i = 0; i < IngredientCount; i++) {
                Ingredients[i].Serialize(stream);
            }
        }
    }

    public static Recipe Deserialize(PhotonStream stream) {
        Recipe r = null;
        if (stream.IsReading) {
            r = new Recipe();

            r.RecipeName = stream.ReceiveNext<string>();
            int IngredientCount = stream.ReceiveNext<int>();
            r.Ingredients = new List<Recipe.Ingredient>();
            for (int i = 0; i < IngredientCount; i++) {
                Recipe.Ingredient ing = Recipe.Ingredient.Deserialize(stream);
                r.Ingredients.Add(ing);
            }
        }
        return r;
    }

    public class Ingredient {
        public string IngredientName;
        public bool IsChopped;
        public CookingState CookingState;

        public void Serialize(PhotonStream stream) {
            if (stream.IsWriting) {
                stream.SendNext(IngredientName);
                stream.SendNext(IsChopped);
                stream.SendNext(CookingState);
            }
        }

        public static Recipe.Ingredient Deserialize(PhotonStream stream) {
            Recipe.Ingredient i = null;
            if (stream.IsReading) {
                i = new Recipe.Ingredient();

                i.IngredientName = stream.ReceiveNext<string>();
                i.IsChopped = stream.ReceiveNext<bool>();
                i.CookingState = stream.ReceiveNext<CookingState>();
            }
            return i;
        }
    }
}
