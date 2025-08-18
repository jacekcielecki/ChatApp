namespace ChatApp.Users.Core.Search;

public class GetUsersBySearchPhrase
{
    private readonly GetUsersBySearchPhraseRepository _getUsersBySearchPhraseRepository;

    public GetUsersBySearchPhrase(GetUsersBySearchPhraseRepository getUsersBySearchPhraseRepository)
    {
        _getUsersBySearchPhraseRepository = getUsersBySearchPhraseRepository;
    }

    public async Task<string[]> Get(string searchPhrase)
    {
        var emails = await _getUsersBySearchPhraseRepository.Get(searchPhrase);
        return emails;
    }
}
