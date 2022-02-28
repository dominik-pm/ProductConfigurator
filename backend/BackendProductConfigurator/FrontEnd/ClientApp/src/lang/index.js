import en from './en.json'
import de from './de.json'
import fr from './fr.json'

export const languageNames = {
    EN: 'en',
    DE: 'de',
    FR: 'fr'
}
export const defaultLang = languageNames.EN

const languages = {
    'en': en,
    'de': de,
    'fr': fr
}

export const translate = (key, language = defaultLang) => {
    let langData = languages[language]

    if (!langData) {
        console.log(`Trying to translate ${key} to a language that doesn't exist: '${language}'`)
        return
    }

    const translation = langData[key]

    if (!translation) {
        // console.log(`There is no translation for ${key} in language ${language}!`)

        // if there is a translation in the default language, at least return that
        if (languages[defaultLang][key]) {
            return languages[defaultLang][key]
        }

        // there is no translation for this key (not even in the default language) -> just return the key
        return key
    }

    return translation
}