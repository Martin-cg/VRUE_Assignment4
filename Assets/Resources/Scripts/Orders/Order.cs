using ExitGames.Client.Photon;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Order {
    private long _ExpiryTime;
    private Recipe _Recipe;
    private long _CreationTimestamp;

    public long ExpiryTime {
        get {
            return _ExpiryTime;
        }
        set {
            _ExpiryTime = value;
        }
    }

    public Recipe Recipe {
        get {
            return _Recipe;
        }
        set {
            _Recipe = value;
        }
    }

    public long CreationTimestamp {
        get {
            return _CreationTimestamp;
        }
        set { }
    }

    public bool HasExpired {
        get {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds() - (CreationTimestamp + ExpiryTime) >= 0;
        }
        set { }
    }

    public float ExpirationProgress {
        get {
            return Math.Clamp((DateTimeOffset.Now.ToUnixTimeMilliseconds() - CreationTimestamp) / (float)ExpiryTime, 0.0F, 1.0F);
        }
        set { }
    }

    public Order(Recipe Recipe, long ExpiryTime) {
        this._ExpiryTime = ExpiryTime;
        this._Recipe = Recipe;
        this._CreationTimestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
    }
    public Order(Recipe Recipe, long ExpiryTime, long CreationTimestamp) {
        this._ExpiryTime = ExpiryTime;
        this._Recipe = Recipe;
        this._CreationTimestamp = CreationTimestamp;
    }

    public void Serialize(PhotonStream stream) {
        if (stream.IsWriting) {
            stream.SendNext(CreationTimestamp);
            stream.SendNext(ExpiryTime);
            Recipe.Serialize(stream);
        }
    }

    public static Order Deserialize(PhotonStream stream) {
        Order o = null;
        if (stream.IsReading) {
            long CreationTimestamp = stream.ReceiveNext<long>();
            long ExpiryTime = stream.ReceiveNext<long>();
            Recipe r = Recipe.Deserialize(stream);

            o = new Order(r, ExpiryTime, CreationTimestamp);
        }
        return o;
    }
}
