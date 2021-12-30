import en from './en.json'
import de from './de.json'
import fr from './fr.json'
import { defaultLang } from '../state/language/languageSlice'

const languages = {
    'EN': en,
    'DE': de,
    'FR': fr
}

export const translate = (key, language) => {
    let langData = languages[language]

    if (!langData) {
        console.log(`Trying to translate ${key} to a language that doesn't exist: '${language}'`)
        return
    }

    const translation = langData[key]

    if (!translation) {
        // console.log(`There is no translation for ${key} in language ${language}!`)

        // if there is a translation in the default  language, at least return that
        if (languages[defaultLang][key]) {
            return languages[defaultLang][key]
        }

        return key
    }

    return translation
}