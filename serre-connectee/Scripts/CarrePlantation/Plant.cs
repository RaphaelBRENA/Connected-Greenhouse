using System;
using Godot;


//CLASSE PARENT _______________________________________________________________________________________


public partial class Plant : Object
{

    //ATTRIBUTS _______________________________________________________________________________________

    private string PlantName;

    private int PlantStage;
    public static Godot.Collections.Array<String> StagesArray = new Godot.Collections.Array<String>(){"Graine","Stade1","Stade2","Stade3","Stade4","Recoltable"};
    private Texture2D Image;

    private int DaysPerStageSick;
    private int DaysPerStageHealthy;
    private int DaysPerStage; //Current numbers of days to change stage

    private int HumidityMin;
    private int HumidityMax;

    private int TemperatureMin;
    private int TemperatureMax;

    private int LuminosityMin;
    private int LuminosityMax;

    private const int MAXDAYSINTOXICATED = 20;

    private bool Dead = false;
    private int MaxDaysAlive;
    private int TotalDaysNumber; //Total of days the plant has lived
    private int TotalChemicalsDaysNumber;
    private int TotalDaysSinceLastStage;



    //CONSTRUCTEUR _______________________________________________________________________________________


    public Plant(string P){
        //Constructeur de la classe Plant
        PlantName = P;
        PlantStage = 0;
        TotalDaysNumber = 0;
        TotalChemicalsDaysNumber = 0;
        TotalDaysSinceLastStage = 0;
        SetImage();

        //données prises et adaptées au code depuis le site web : https://www.fermedesaintemarthe.com/

        if(PlantName=="Graines Haricot"){ // HARICOT NAIN BRAIMAR MANGETOUT AB
            HumidityMin = 50;
            HumidityMax = 60;
            TemperatureMin = 22;
            TemperatureMax = 25;
            LuminosityMin = 12500;
            LuminosityMax = 35000;
            DaysPerStageHealthy = 3;
        }
		if(PlantName=="Graines Ciboulette"){ // CIBOULETTE COMMUNE AB
            HumidityMin = 50;
            HumidityMax = 60;
            TemperatureMin = 15 ;
            TemperatureMax = 20 ;
            LuminosityMin = 8750;
            LuminosityMax = 25000;
            DaysPerStageHealthy = 3;
        }
		if(PlantName=="Graines Lavande"){ // LAVANDE OFFICINALE AB
            HumidityMin = 10 ;
            HumidityMax = 30 ;
            TemperatureMax = 25 ;
            LuminosityMin = 12500;
            LuminosityMax = 35000;
            DaysPerStageHealthy = 10;
        }

        if(PlantName=="Graines Lentille"){ // LENTILLE COMMUNE BLONDE AB
            HumidityMin = 75;
            HumidityMax = 80;
            TemperatureMin = 18;
            TemperatureMax = 22;
            LuminosityMin = 12500;
            LuminosityMax = 35000;
            DaysPerStageHealthy = 2;
        }

		if(PlantName=="Graines Basilic"){ // BASILIC GREC AB
            HumidityMin = 50;
            HumidityMax = 60;
            TemperatureMin = 18;
            TemperatureMax = 22;
            LuminosityMin = 8750;
            LuminosityMax = 25000;
            DaysPerStageHealthy = 3;
        }
		if(PlantName=="Graines Origan"){ // ORIGAN AB
            HumidityMin = 30;
            HumidityMax = 50;
            TemperatureMin = 20;
            TemperatureMax = 25;
            LuminosityMin = 12500;
            LuminosityMax = 35000;
            DaysPerStageHealthy = 4;
        }
		if(PlantName=="Graines Menthe"){ // MENTHE VERTE AB
            HumidityMin = 40;
            HumidityMax = 60;
            TemperatureMin = 18;
            TemperatureMax = 22;
            LuminosityMin = 8750;
            LuminosityMax = 25000;
            DaysPerStageHealthy = 4;
        }
		if(PlantName=="Graines Sauge"){ // SAUGE OFFICINALE AB
            HumidityMin = 10;
            HumidityMax = 30;
            TemperatureMin = 10;
            TemperatureMax = 20;
            LuminosityMin = 12500;
            LuminosityMax = 35000;
            DaysPerStageHealthy = 4;
        }
		if(PlantName=="Graines Persil"){ // PERSIL PLAT COMMUN 2 AB
            HumidityMin = 40;
            HumidityMax = 60;
            TemperatureMin = 18;
            TemperatureMax = 20;
            LuminosityMin = 8750;
            LuminosityMax = 25000;
            DaysPerStageHealthy = 6;
        }
		if(PlantName=="Graines Aneth"){ // ANETH BOUQUET AB
            HumidityMin = 40;
            HumidityMax = 60;
            TemperatureMin = 18;
            TemperatureMax = 22;
            LuminosityMin = 12500;
            LuminosityMax = 35000;
            DaysPerStageHealthy = 5;
        }
		if(PlantName=="Graines Cosmos"){ // COSMOS SENSATION VARIE AB
            HumidityMin = 50;
            HumidityMax = 60;
            TemperatureMin = 20;
            TemperatureMax = 25;
            LuminosityMin = 12500;
            LuminosityMax = 35000;
            DaysPerStageHealthy = 5;
        }
        DaysPerStageSick = DaysPerStageHealthy + 2;
        MaxDaysAlive = (int)(DaysPerStageHealthy*8.5);
    }

    //ACCESSEURS _______________________________________________________________________________________


    public string GetName(){
        return PlantName;
    }

    public string GetStade(){
        return StagesArray[PlantStage];
    }

    public Texture2D GetImage(){
        return Image;
    }

	public int GetPlantStage(){
        return PlantStage;
    }

    public int GetDaysPerStage(){
        return DaysPerStageHealthy;
    }

    public int GetTotalDaysNumber(){
        return TotalDaysNumber;
    }

    public int GetTotalChemicalDaysNumber(){
        return TotalChemicalsDaysNumber;
    }

    public int GetTotalDaysSinceLastStage(){
        return TotalDaysSinceLastStage;
    }

    public int GetMaxHumidity(){
        return HumidityMax;
    }

    public int GetHumidityMin(){
        return HumidityMin;
    }

    public int GetTemperatureMax(){
        return TemperatureMax;
    }

    public int GetTemperatureMin(){
        return TemperatureMin;
    }


    public string GetOptimalHumidity(){
        string Plage = "entre "+HumidityMin+"% et "+HumidityMax+"%";
        return Plage;
    }
    public string GetOptimalTemperature(){
        string Plage = "entre "+TemperatureMin+"°C et "+TemperatureMax+"°C";
        return Plage;
    }
    public string GetOptimalLuminosity(){
        string Plage = "entre "+LuminosityMin+" lux et "+LuminosityMax+" lux";
        return Plage;
    }

    public bool GetDead(){
        return Dead;
    }

   	//METHODES DE SAUVEGARDE __________________________________________________________________________________

    public void LoadPlant(int SavedPlantStage, int SavedTotalDaysNumber, int SavedTotalChemicalsDaysNumber, int SavedTotalDaysSinceLastStage, bool IsDead){
        //Fonction de chargement de la plante depuis les données sauvegardées
        PlantStage = SavedPlantStage;
        TotalDaysNumber = SavedTotalDaysNumber;
        TotalChemicalsDaysNumber = SavedTotalChemicalsDaysNumber;
        TotalDaysSinceLastStage = SavedTotalDaysSinceLastStage;
        Dead = IsDead;
        SetImage();
    }

    //METHODES ________________________________________________________________________________________________

    public void BecomesSick(bool Value){
        if(Value){
             DaysPerStage = DaysPerStageSick;
        }else{
             DaysPerStage = DaysPerStageHealthy;
        }
    }


    //METHODES ________________________________________________________________________________________________

    public void NewDay(double Humidity, int Nitrogen, double Temperature, int Luminosity,bool ChemicalIntoxication){
        //Appelée lorsqu'un nouveau jour commence. Incrémente les attributs de jour
        //Et voit si la plante peut passer à son stade suivant.
        TotalDaysSinceLastStage++;
        if(PlantStage!=0)
        {
            TotalDaysNumber++;
        }
        if(ChemicalIntoxication){
            DaysPerStage = DaysPerStageSick ; //Si intoxiquée, elle pousse plus lentement
            TotalChemicalsDaysNumber++;
        }else{
            DaysPerStage = DaysPerStageHealthy;
            TotalChemicalsDaysNumber = 0;
        }

        NextStage(Humidity, Nitrogen, Temperature, Luminosity, ChemicalIntoxication);
    }

    private void NextStage(double Humidity, int Nitrogen, double Temperature, int Luminosity,bool ChemicalIntoxication){
        //Fait passer la plante au stade suivant si les paramètres correspondent.
        if(TotalDaysNumber >= MaxDaysAlive || TotalChemicalsDaysNumber >= MAXDAYSINTOXICATED){
            Dead = true;
            SetImage();
        }
        else{
            bool PeutEvoluer = true;
            if(Humidity<HumidityMin || Humidity>HumidityMax ||
               Temperature<TemperatureMin || Temperature>TemperatureMax ||
               Luminosity<LuminosityMin || Luminosity>LuminosityMax ||
               Dead || TotalDaysSinceLastStage<DaysPerStage){
               PeutEvoluer = false;
            }
            if(PeutEvoluer &&  StagesArray[PlantStage]!="Recoltable"){
                TotalDaysSinceLastStage = 0;
                PlantStage++;
                SetImage();
            }
        }
        //ICI : l'Azote n'est pas encore implémenté dans le jeu, mais presque tout est prêt pour.
    }

    private void SetImage(){
        //Attribue l'image en fonction du stade de croissance
        if (Dead) {
            Image = (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/"+(PlantName.Substring(8,PlantName.Length-8)+"Morte.png"));
        } else if(StagesArray[PlantStage]=="Recoltable"){
           Image = (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/"+(PlantName.Substring(8,PlantName.Length-8))+".png");
        } else{
            Image = (Texture2D) ResourceLoader.Load("../../Assets/Images/ImagesObjets/"+(PlantName.Substring(8,PlantName.Length-8)+StagesArray[PlantStage])+".png");
        }

	}

    public string IsGatherable(){
        //Si la plante est récoltable, renvoie le nom du Product (ex: Haricot)
        //Sinon si la plante est morte, renvoie Morte
        //Sinon, renvoie En Croissance
        if(StagesArray[PlantStage]=="Recoltable" && !Dead){
            return PlantName.Substring(8,PlantName.Length-8);
        }else if(Dead){
            return "Morte";
        }else{
            return "En Croissance";
        }
    }
}
