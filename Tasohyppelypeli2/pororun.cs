using System;
using System.Collections.Generic;
using System.Net.Mime;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace pororun;

/// @author Omanimi
/// @version 03.03.2024
/// <summary>
/// 
/// </summary>
public class pororun : PhysicsGame
{
    private const double NOPEUS = 10000;
    private const double HYPPYNOPEUS = 750;
    private const int RUUDUN_KOKO = 40;
    private PlatformCharacter pelaaja1;
    private Image pelaajanKuva = LoadImage("hahmo.png");
    private Image tahtiKuva = LoadImage("salmiakkia.png");
    private Timer liikutusajastin;
    private Image  vihollinenkuva= LoadImage("vihu.png");
    private SoundEffect maaliAani = LoadSoundEffect("maali.wav");
    private bool peliKaynnissa = false;
    public override void Begin()
    {
        Gravity = new Vector(0, -1000);

        LuoKentta();
        LisaaNappaimet();

        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = true;

        MasterVolume = 0.5;
        liikutusajastin = new Timer();
        liikutusajastin.Interval = 0.01;
        liikutusajastin.Timeout += SiirraPelaajaaOikeammalle;
        liikutusajastin.Start();
        peliKaynnissa = true;
    }
    
    void SiirraPelaajaaOikeammalle()
    {
        pelaaja1.Push(new Vector(NOPEUS, 0.0));
    }
    private void LuoKentta()
    {
        TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('J', LisaaPelaaja);
        kentta.SetTileMethod('v', LisaaVihollinen); 
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.White, Color.SkyBlue);
        Camera.FollowOffset = new Vector(Screen.Width / 2.5 - RUUDUN_KOKO, 0.0);
    }

    private void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.Green;
        taso.Tag = "seina";
        Add(taso);
    }

    private void LisaaTahti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tahti.IgnoresCollisionResponse = true;
        tahti.Position = paikka;
        tahti.Image = tahtiKuva;
        tahti.Tag = "tahti";
        Add(tahti);
    }

    void TormaaKuolemaan(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        if (peliKaynnissa)
        {
            MessageDisplay.Add("löllöllöö! :(");
            Keyboard.Disable(Key.Up);
            peliKaynnissa = false;
            liikutusajastin.Stop();
        }
    }
    private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus);
        pelaaja1.Position = paikka;
        pelaaja1.Mass = 4.0;
        pelaaja1.Image = pelaajanKuva;
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        AddCollisionHandler(pelaaja1, "seina", TormaaKuolemaan);
        AddCollisionHandler(pelaaja1, "vihu", TormaaKuolemaan);
        Add(pelaaja1);
    }

    void LisaaVihollinen(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject vihollinen = new PhysicsObject(leveys, korkeus);
        vihollinen.Position = paikka;
        Add(vihollinen);
        vihollinen.Image = vihollinenkuva;
        vihollinen.IgnoresGravity = true;
        vihollinen.CanRotate = false;
        vihollinen.IgnoresCollisionResponse = true;
        vihollinen.Oscillate(new Vector(0, 1), korkeus * 1.5, 0.3);
        vihollinen.Tag = "vihu";
    }

    private void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
        
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    private void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
    }

    private void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.ForceJump(nopeus);
    }

    private void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
    {
        maaliAani.Play();
        MessageDisplay.Add("Keräsit tähden!");
        tahti.Destroy();
    }
    private void UusiEste(Vector paikka, double leveys, double korkeus) 
    {
        
    }
}