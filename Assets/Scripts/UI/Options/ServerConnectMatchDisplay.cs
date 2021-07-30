using MatchUp;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerConnectMatchDisplay : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private TMP_Text gameName;
    [SerializeField] private TMP_Text region;
    [SerializeField] private TMP_Text playercount;
    [SerializeField] private Image passwordProtected;

    private ServerConnect serverConnect;
    private Match match;

    private bool isPasswordProtected;
    private string matchName;
    private int connected;
    private int maxPlayers;
    private string regionString;

    public bool Set(Match match, ServerConnect serverConnect)
    {
        if (match.matchData.TryGetValue("Match name", out MatchData nameData) == false ||
            match.matchData.TryGetValue("Max players", out MatchData playersData) == false ||
            match.matchData.TryGetValue("Connected players", out MatchData connectedData) == false ||
            match.matchData.TryGetValue("Region", out MatchData regionData) == false ||
            match.matchData.TryGetValue("Password protected", out MatchData passwordData) == false)
            return false;

        if (nameData.valueType != MatchData.ValueType.STRING ||
            playersData.valueType != MatchData.ValueType.INT ||
            connectedData.valueType != MatchData.ValueType.INT ||
            regionData.valueType != MatchData.ValueType.INT ||
            passwordData.valueType != MatchData.ValueType.INT)
            return false;

        matchName = nameData.stringValue;
        if (matchName.Length > 20)
            matchName = matchName.Substring(0, 20);

        connected = connectedData.intValue;
        maxPlayers = playersData.intValue;

        if (connected < 1 || connected >= maxPlayers || maxPlayers > 4)
            return false;

        isPasswordProtected = passwordData.intValue != 0;

        int regionVal = regionData.intValue;
        if (regionVal < 1 || regionVal > 7)
            return false;

        NobleConnect.GeographicRegion geoRegion = (NobleConnect.GeographicRegion)regionVal;
        regionString = GeographicRegionDict.Instance.GetName(geoRegion);
        if (regionString == default)
            return false;

        gameName.text = matchName;
        Color passwordColor = passwordProtected.color;
        passwordColor.a = isPasswordProtected ? 1 : 0;
        passwordProtected.color = passwordColor;
        region.text = regionString;
        playercount.text = connected + "/" + maxPlayers;
        
        this.serverConnect = serverConnect;
        this.match = match;

        return true;
    }

    public void OnPointerClick(PointerEventData eventData) => 
        serverConnect.OnMatchClicked(match, isPasswordProtected, matchName, connected, maxPlayers, regionString);

}
