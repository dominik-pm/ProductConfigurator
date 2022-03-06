import { createSlice } from '@reduxjs/toolkit'
import { fetchId, postConfiguration, putConfiguration } from '../../api/configurationAPI'
import { setAcceptLanguage } from '../../api/general'
import { fetchAvailableImages } from '../../api/productsAPI'
import { readFromLocalStorage, writeToLocalStorage } from '../../App'
import { defaultLang, languageNames } from '../../lang'
import { extractGroupsFromBuilderSection, extractModelOptionsFromBuilderModel, extractOptionsFromBuilderGroup, getBuilderGroupById, getBuilderSectionById, getDoesGroupdExist, getDoesOptionExist, getDoesSectionExist, selectBuilderGroupRequirements, selectBuilderModels, selectBuilderOptionIncompatibilities, selectBuilderOptionRequirements, selectBuilderConfiguration, getBuilderGroupIdByOptionId } from './builderSelectors'


const initialConfiguration = {
    configId: '',
    images: [],
    options: [],
    optionSections: [],
    optionGroups: [],
    rules: {
        basePrice: 0,
        defaultModel: '',
        models: [],
        groupRequirements: {
            // PANORAMATYPE_GROUP: ['PANORAMA_GROUP']
        },
        requirements: {
            // BLUE: ['STEEL16'],
        },
        incompatibilities: {
            // PANORAMAROOF: ['PETROL']
        },
        priceList: {
            // BLUE: 200
        }
    },
    languages: {
        en: {
            name: '',
            description: '',
            options: [],
            optionSections: [],
            optionGroups: [],
            models: []
        },
        de: {
            name: '',
            description: '',
            options: [],
            optionSections: [],
            optionGroups: [],
            models: []
        },
        fr: {
            name: '',
            description: '',
            options: [],
            optionSections: [],
            optionGroups: [],
            models: []
        }
    }
}

const testConfiguration = {
    configId: 'TestNeueKonfig_-_ENTitle',
    images: [],
    options: [
        {
            id: 'ALLOY19',
            groupId: 'WHEELS2',
            productNumber: 'TN1'
        },
        {
            id: 'STEEL16',
            groupId: 'WHEELS2',
            productNumber: 'TN1'
        },
        {
            id: 'RED',
            groupId: 'COLOR_GROUP2',
            productNumber: 'TN1'
        },
        {
            id: 'BLUE222',
            groupId: 'COLOR_GROUP2',
            productNumber: 'TN1'
        }
    ],
    optionSections: [
        {
            id: 'EXTERIOR2',
            optionGroupIds: [
                'WHEELS2', 'COLOR_GROUP2'
            ]
        }
    ],
    optionGroups: [
        {
            id: 'WHEELS2',
            optionIds: [
                'ALLOY19', 'STEEL16'
            ],
            required: true,
            replacement: true
        },
        {
            id: 'COLOR_GROUP2',
            optionIds: [
                'BLUE222', 'RED'
            ],
            required: false,
            replacement: false
        }
    ],
    rules: {
        basePrice: 500,
        defaultModel: '',
        models: [
            {
                id: 'Sport',
                optionIds: ['ALLOY19', 'RED'],
            },
            {
                id: 'Basic',
                optionIds: ['STEEL16', 'BLUE222'],
            }
        ],
        // replacementGroups: {
        //     // COLOR_GROUP2: [
        //     //     'BLUE222'
        //     // ]
        // },
        groupRequirements: {
            // PANORAMATYPE_GROUP: ['PANORAMA_GROUP']
            COLOR_GROUP2: ['WHEELS2'],
            WHEELS2: ['COLOR_GROUP2']
        },
        requirements: {
            BLUE222: ['STEEL16'],
            ALLOY19: ['RED']
        },
        incompatibilities: {
            // PANORAMAROOF: ['PETROL']
            BLUE222: ['ALLOY19'],
            STEEL16: ['RED']
        },
        priceList: {
            'BLUE222': 200
        }
    },
    languages: {
        en: {
            name: 'TestNeueKonfig - ENTitle',
            description: 'test en desc',
            options: [
                {
                    id: 'ALLOY19',
                    name: '19 inch Alloy',
                    description: 'description',
                },
                {
                    id: 'STEEL16',
                    name: '16 inch Steel',
                    description: 'description',
                },
                {
                    id: 'RED',
                    name: 'red',
                    description: 'Farbe rot',
                },
                {
                    id: 'BLUE222',
                    name: 'blue',
                    description: 'Farbe blau',
                }
            ],
            optionSections: [
                {
                    id: 'EXTERIOR2',
                    name: 'Exterior'
                }
            ],
            optionGroups: [
                {
                    id: 'WHEELS2',
                    name: 'Wheels',
                    description: 'round stuff'
                },
                {
                    id: 'COLOR_GROUP2',
                    name: 'Color',
                    description: 'of the car',
                }
            ],
            models: [
                {
                    id: 'Sport',
                    name: 'Sport',
                    description: 'en description, description, description, description, description, description, description, description, description, description, description, description, description, description, description, description,'
                },
                {
                    id: 'Basic',
                    name: 'Basic',
                    description: 'en description, description, description, description, description, description, description, description, '
                }
            ]
        },
        de: {
            name: 'TestNeueKonfig - DETitle',
            description: 'test de desc',
            options: [
                {
                    id: 'ALLOY19',
                    name: '19 zoll Alo',
                    description: 'Beschreibung',
                },
                {
                    id: 'STEEL16',
                    name: '16 zoll Stahl',
                    description: 'Beschreibung',
                },
                {
                    id: 'RED',
                    name: 'red',
                    description: 'Farbe rot',
                },
                {
                    id: 'BLUE222',
                    name: 'blau',
                    description: 'Farbe blau',
                }
            ],
            optionSections: [
                {
                    id: 'EXTERIOR2',
                    name: 'Außen'
                }
            ],
            optionGroups: [
                {
                    id: 'WHEELS2',
                    name: 'Reifen',
                    description: 'Reifen halt'
                },
                {
                    id: 'COLOR_GROUP2',
                    name: 'Farbe',
                    description: 'Farbe vom Auto'
                }
            ],
            models: [
                {
                    id: 'Sport',
                    name: 'Sport',
                    description: 'Beschreibung Sport'
                },
                {
                    id: 'Basic',
                    name: 'Standard',
                    description: 'Beschreibung Standard'
                }
            ]
        },
        fr: {
            name: 'TestNeueKonfig - FRTitle',
            description: 'test fr desc',
            options: [
                {
                    id: 'ALLOY19',
                    name: '19 inch Alloy',
                    description: 'description',
                },
                {
                    id: 'STEEL16',
                    name: '16 inch Steel',
                    description: 'description',
                },
                {
                    id: 'RED',
                    name: 'red',
                    description: 'Farbe rot',
                },
                {
                    id: 'BLUE222',
                    name: 'blue',
                    description: 'Farbe blau',
                }
            ],
            optionSections: [
                {
                    id: 'EXTERIOR2',
                    name: 'Exterior'
                }
            ],
            optionGroups: [
                {
                    id: 'WHEELS2',
                    name: 'Wheels',
                    description: 'round stuff'
                },
                {
                    id: 'COLOR_GROUP2',
                    name: 'Color',
                    description: 'of the car',
                }
            ],
            models: [
                {
                    id: 'Sport',
                    name: 'Sport',
                    description: 'description fr'
                },
                {
                    id: 'Basic',
                    name: 'Basic',
                    description: 'description fr'
                }
            ]
        }
    }
}

const initialState = {
    configuration: testConfiguration, // initialConfiguration
    currentLanguage: defaultLang,
    availableImages: [],
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const builderSlice = createSlice({
    name: 'builder',
    initialState: {
        ...initialState, 
        configuration: readFromLocalStorage('builder') || initialState.configuration
    },
    reducers: {
        setImages: (state, action) => {
            // only add to images if it is available (the set image could be from local storage and the available images could have changed)
            state.configuration.images = []
            action.payload.forEach(newImage => {
                if (state.availableImages.includes(newImage)) {
                    state.configuration.images.push(newImage)
                }
            })
        },
        setAvailableImages: (state, action) => {
            state.availableImages = action.payload
        },
        addSection: (state, action) => {
            const name = action.payload

            // add to sections
            state.configuration.optionSections.push({
                id: name,
                // name: name,
                optionGroupIds: []
            })

            // add to every language
            const newSection = { id: name, name }
            for (const lang in state.configuration.languages) {
                const langObj = state.configuration.languages[lang]
                const sections = langObj.optionSections
                sections.push(newSection)
            }
            // const section = sections.find(s => s.id === name)
            // if (section) section.name = name
            // else sections.push({id: name, name: name})

            // const sections = state.configuration.languages[language].optionSections
            // const section = sections.find(s => s.id === name)
            // if (section) section.name = name
            // else sections.push({id: name, name: name})

            // state.configuration.languages[language].optionSections[name] = {name: name}
        },
        changeSectionProperties: (state, action) => {
            const { sectionId, newName } = action.payload

            const section = state.configuration.languages[state.currentLanguage].optionSections.find(s => s.id === sectionId)
            if (section) {
                section.name = newName || section.name
            }
        },
        removeSection: (state, action) => {
            const sectionId = action.payload

            // remove from sections
            state.configuration.optionSections = state.configuration.optionSections.filter(s => s.id !== sectionId)
            
            // remove from all languages
            for (const lang in state.configuration.languages) {
                const langObj = state.configuration.languages[lang]
                langObj.optionSections = langObj.optionSections.filter(s => s.id !== sectionId)
            }
        },
        addOptionGroup: (state, action) => {
            const { sectionId, groupId, name, description, isRequired, isReplacementGroup } = action.payload

            state.configuration.optionGroups.push({
                id: groupId,
                // name: name,
                // description: description,
                required: isRequired,
                replacement: isReplacementGroup,
                optionIds: []
            })

            // add to every language
            const newGroup = {
                id: groupId, 
                name, 
                description
            }
            for (const lang in state.configuration.languages) {
                const langObj = state.configuration.languages[lang]
                const groups = langObj.optionGroups
                groups.push(newGroup)
            }
            
            // add to language
            // const groups = state.configuration.languages[state.currentLanguage].optionGroups
            // let group = groups.find(g => g.id === groupId)
            // const updatedGroup = {
            //     id: groupId, 
            //     name, 
            //     description
            // }
            // if (group) group = updatedGroup
            // else groups.push(updatedGroup)

            // add to section
            const section = state.configuration.optionSections.find(s => s.id === sectionId)
            if (section) section.optionGroupIds.push(groupId)
        },
        changeGroupProperties: (state, action) => {
            const { groupId, newName, newDescription } = action.payload

            const group = state.configuration.languages[state.currentLanguage].optionGroups.find(g => g.id === groupId)
            if (group) {
                group.name = newName || group.name
                group.description = newDescription || group.description
            }
        },
        setGroupRequirements: (state, action) => {
            const { groupId, requirements } = action.payload

            state.configuration.rules.groupRequirements[groupId] = requirements

            // cleanup empty requirements (remove dictionary entry if the option array is empty)
            for (const gId in state.configuration.rules.groupRequirements) {
                if (state.configuration.rules.groupRequirements[gId].length === 0) {
                    delete state.configuration.rules.groupRequirements[gId]
                }
            }
        },
        setGroupIsRequired: (state, action) => {
            const { groupId, required } = action.payload

            const group = state.configuration.optionGroups.find(g => g.id === groupId)
            if (group) group.required = required
        },
        setGroupIsReplacement: (state, action) => {
            const { groupId, replacement } = action.payload

            const group = state.configuration.optionGroups.find(g => g.id === groupId)
            if (group) group.replacement = replacement
        },
        removeOptionGroup: (state, action) => {
            const { groupId, sectionId } = action.payload

            // remove group requirements
            if (state.configuration.rules.groupRequirements[groupId]) delete state.configuration.rules.groupRequirements[groupId]

            // remove group from group list
            state.configuration.optionGroups = state.configuration.optionGroups.filter(g => g.id !== groupId)

            // remove from all languages
            for (const lang in state.configuration.languages) {
                const langObj = state.configuration.languages[lang]
                langObj.optionGroups = langObj.optionGroups.filter(g => g.id !== groupId)
            }

            // remove group from section
            const section = state.configuration.optionSections.find(s => s.id === sectionId)
            if (section) section.optionGroupIds = section.optionGroupIds.filter(g => g !== groupId)
        },
        addOption: (state, action) => {
            const { groupId, optionId, name, description, productNumber, price } = action.payload

            // add option to options list
            state.configuration.options.push({
                id: optionId,
                productNumber: productNumber,
                // name: name,
                // description: description,
                groupId: groupId
            })

            // add to every language
            const newOption = {
                id: optionId, 
                name, 
                description
            }
            for (const lang in state.configuration.languages) {
                const langObj = state.configuration.languages[lang]
                const options = langObj.options
                options.push(newOption)
            }

            // add to language
            // const options = state.configuration.languages[language].options
            // let option = options.find(o => o.id === optionId)
            // const updatedOption = {
            //     id: optionId,
            //     name,
            //     description
            // }
            // if (option) option = updatedOption
            // else options.push(updatedOption)

            // add option to group
            const group = state.configuration.optionGroups.find(g => g.id === groupId)
            if (group) group.optionIds.push(optionId)

            // add option price to pricelist in rules
            if (price) state.configuration.rules.priceList[optionId] = price
        },
        changeOptionProperties: (state, action) => {
            const { optionId, newName, newDescription, newProductNumber } = action.payload

            const langOption = state.configuration.languages[state.currentLanguage].options.find(o => o.id === optionId)
            if (langOption) {
                langOption.name = newName || langOption.name
                langOption.description = newDescription || langOption.description
            }
            
            const option = state.configuration.options.find(o => o.id === optionId)
            if (option) {
                option.productNumber = newProductNumber || option.productNumber
            }
        },
        setOptionPrice: (state, action) => {
            const { optionId, price } = action.payload

            state.configuration.rules.priceList[optionId] = price
        },
        setOptionRequirements: (state, action) => {
            const { optionId, requirements } = action.payload

            state.configuration.rules.requirements[optionId] = requirements

            // cleanup empty requirements (remove dictionary entry if the option array is empty)
            for (const oId in state.configuration.rules.requirements) {
                if (state.configuration.rules.requirements[oId].length === 0) {
                    delete state.configuration.rules.requirements[oId]
                }
            }
        },
        setOptionIncompatibilities: (state, action) => {
            const { optionId, incompatibilities } = action.payload

            state.configuration.rules.incompatibilities[optionId] = incompatibilities

            // cleanup empty incompatitbilities (remove dictionary entry if the option array is empty)
            for (const oId in state.configuration.rules.incompatibilities) {
                if (state.configuration.rules.incompatibilities[oId].length === 0) {
                    delete state.configuration.rules.incompatibilities[oId]
                }
            }
        },
        removeOption: (state, action) => {
            const { groupId, optionId } = action.payload

            // remove option from option list
            state.configuration.options = state.configuration.options.filter(o => o.id !== optionId)

            // remove option from group
            const group = state.configuration.optionGroups.find(g => g.id === groupId)
            if (group) group.optionIds = group.optionIds.filter(o => o !== optionId)

            // remove from all languages
            for (const lang in state.configuration.languages) {
                const langObj = state.configuration.languages[lang]
                langObj.options = langObj.options.filter(o => o.id !== optionId)
            }

            // remove option requirements
            if (state.configuration.rules.requirements[optionId]) delete state.configuration.rules.requirements[optionId]

            // remove option incompatibilities
            if (state.configuration.rules.incompatibilities[optionId]) delete state.configuration.rules.incompatibilities[optionId]

            // remove option price from pricelist
            if (state.configuration.rules.priceList[optionId]) delete state.configuration.rules.priceList[optionId]
        },
        addModel: (state, action) => {
            const { modelName, options, description } = action.payload

            state.configuration.rules.models.push({
                id: modelName, 
                optionIds: options, 
            })

            // add to every language
            const newModel = {
                id: modelName,
                name: modelName,
                description
            }
            for (const lang in state.configuration.languages) {
                const langObj = state.configuration.languages[lang]
                const models = langObj.models
                models.push(newModel)
            }

            // add to language
            // const models = state.configuration.languages[state.currentLanguage].models
            // let model = models.find(m => m.id === modelName)
            // const updatedModel = {
            //     id: modelName,
            //     name: modelName,
            //     description
            // }
            // if (model) model = updatedModel
            // else models.push(updatedModel)
        },
        changeModelProperties: (state, action) => {
            const { modelId, newName, newDescription } = action.payload

            const model = state.configuration.languages[state.currentLanguage].models.find(m => m.id === modelId)
            if (model) {
                model.name = newName || model.name
                model.description = newDescription || model.description
            }
        },
        setDefaultModel: (state, action) => {
            state.configuration.rules.defaultModel = action.payload
        },
        setModelOptions: (state, action) => {
            const { modelId, options } = action.payload
            
            const model = state.configuration.rules.models.find(m => m.id === modelId)
            if (model) model.optionIds = options
        },
        removeModel: (state, action) => {
            const modelId  = action.payload

            // remove from models
            state.configuration.rules.models = state.configuration.rules.models.filter(m => m.id !== modelId)

            // remove from languages
            for (const lang in state.configuration.languages) {
                const langObj = state.configuration.languages[lang]
                langObj.models = langObj.models.filter(m => m.id !== modelId)
            }
        },
        setBasePrice: (state, action) => {
            state.configuration.rules.basePrice = action.payload
        },
        setDescription: (state, action) => {
            const description = action.payload

            state.configuration.languages[state.currentLanguage].description = description

            // also set the description to the other langs if its empty
            for (const lang in state.configuration.languages) {
                if (!state.configuration.languages[lang].description) {
                    state.configuration.languages[lang].description = description
                }
            }
        },
        setName: (state, action) => {
            const name = action.payload.replace('*', '')

            state.configuration.languages[state.currentLanguage].name = name

            // also set the name to the other langs if its empty
            for (const lang in state.configuration.languages) {
                if (!state.configuration.languages[lang].name) {
                    state.configuration.languages[lang].name = name
                }
            }
        },
        changeInputLanguage: (state, action) => {
            const newLanguage = action.payload

            state.currentLanguage = newLanguage
        },
        resetBuild: (state, action) => {
            state.configuration = initialConfiguration
        },
        setConfigurationToEdit: (state, action) => {
            const { configId, name, description, images, options, optionSections, optionGroups, rules, language } = action.payload

            state.configuration.configId = configId
            state.configuration.images = images
            state.configuration.options = options.map(o => {
                return {
                    id: o.id,
                    groupId: o.groupId,
                    productNumber: o.productNumber
                }
            })
            state.configuration.optionSections = optionSections.map(s => {
                return {
                    id: s.id,
                    optionGroupIds: s.optionGroupIds
                }
            })
            state.configuration.optionGroups = optionGroups.map(g => {
                return {
                    id: g.id, 
                    optionIds: g.optionIds,
                    required: g.required,
                    replacement: g.replacement
                }
            })
            state.currentLanguage = language
            state.configuration.languages[language] = {
                name,
                description,
                options: options.map(o => {
                    return {
                        id: o.id,
                        name: o.name,
                        description: o.description,
                    }
                }),
                optionSections: optionSections.map(s => {
                    return {
                        id: s.id,
                        name: s.name,
                    }
                }),
                optionGroups: optionGroups.map(g => {
                    return {
                        id: g.id,
                        name: g.name,
                        description: g.description
                    }
                }),
                models: rules.models.map(m => {
                    return {
                        id: m.id,
                        name: m.name,
                        description: m.description
                    }
                })
            }
            state.configuration.rules = {
                basePrice: rules.basePrice,
                defaultModel: rules.defaultModel,
                models: rules.models.map(m => {
                    return {
                        id: m.id,
                        optionIds: m.optionIds
                    }
                }),
                groupRequirements: rules.groupRequirements,
                requirements: rules.requirements,
                incompatibilities: rules.incompatibilities,
                priceList: rules.priceList
            }
        },
        loadingStarted: (state) => {
            state.status = 'loading'
        },
        loadingSucceeded: (state, action) => {
            state.status = 'succeeded'
        },
        loadingFailed: (state, action) => {
            state.status = 'failed'
            state.error = action.payload
        },
        loadingHandled: (state, action) => {
            state.status = 'idle'
            state.error = null
        }
    },
    // extraReducers: (builder) => {
    //     builder
    //         .addCase(loadingSucceeded, (state, action) => {
    //         })
    // }
})

export const loadImages = () => async (dispatch, getState) => {
    fetchAvailableImages()
    .then(images => {
        dispatch(setAvailableImages(images))
    })
    .catch(err => {
        dispatch(loadingFailed(err))
    })
}

export const saveBuilderToStorage = () => (dispatch, getState) => {
    writeToLocalStorage(selectBuilderConfiguration(getState()), 'builder')
}

export const createSection = (sectionName) => (dispatch, getState) => {
    // check if section doesn't already exist
    const sectionExists = getDoesSectionExist(getState(), sectionName)
    if (sectionExists) {
        return false
    }

    dispatch(addSection(sectionName))
    return true
}
export const deleteSection = (sectionId) => (dispatch, getState) => {
    const section = getBuilderSectionById(getState(), sectionId)

    if (!section) {
        console.log('Could not delete section -> no section matches the id: ' + sectionId)
        return
    }
    
    // remove associated groups
    const groups = extractGroupsFromBuilderSection(section)
    groups.forEach(groupId => {
        dispatch(deleteOptionGroup(groupId))
    })

    // remove section
    dispatch(removeSection(sectionId))
}

export const createGroup = (sectionId, name, description, isRequired, isReplacementGroup) => (dispatch, getState) => {

    const groupId = name.replace(' ', '_')

    // check if section doesn't already exist
    const groupExists = getDoesGroupdExist(getState(), groupId)
    if (groupExists) {
        return false
    }

    dispatch(addOptionGroup({sectionId, groupId, name, description, isRequired, isReplacementGroup}))
    return true
}
export const deleteOptionGroup = (groupId, sectionId) => (dispatch, getState) => {
    const group = getBuilderGroupById(getState(), groupId)

    if (!group) {
        console.log('Could not delete group -> no group matches the id: ' + groupId)
        return
    }

    // remove associated options
    const options = extractOptionsFromBuilderGroup(group)
    options.forEach(optionId => {
        dispatch(deleteOption(groupId, optionId))
    })

    // remove group from other group requirements
    const groups = selectBuilderGroupRequirements(getState())
    for (const gId in groups) {
        const requirements = groups[gId].filter(req => req !== groupId)
        dispatch(setGroupRequirements({groupId: gId, requirements}))
    }

    // remove group
    dispatch(removeOptionGroup({groupId, sectionId}))
}

export const createOption = (groupId, name, description, productNumber, price = 0) => (dispatch, getState) => {
    const optionId = `${name}_${groupId}`.replace(' ', '_')

    // check if option doesn't already exist
    const optionExists = getDoesOptionExist(getState(), optionId)
    if (optionExists) {
        return false
    }

    dispatch(addOption({groupId, optionId, name, description, productNumber, price}))
    return true
}
export const deleteOption = (groupId, name) => (dispatch, getState) => {
    // remove option from models
    const models = selectBuilderModels(getState())
    models.forEach(model => {
        const newOptions = extractModelOptionsFromBuilderModel(model).filter(optionId => optionId !== name) // get the models options and filter out the option that gets removed
        dispatch(changeModelOptions(model.id, newOptions))
    })

    // remove option from other requirements
    const allRequirements = selectBuilderOptionRequirements(getState())
    for (const oId in allRequirements) {
        const requirements = allRequirements[oId].filter(req => req !== name)
        dispatch(setOptionRequirements({optionId: oId, requirements}))
    }

    // remove option from other incompatibilities
    const allIncompatibilities = selectBuilderOptionIncompatibilities(getState())
    for (const oId in allIncompatibilities) {
        const incompatibilities = allIncompatibilities[oId].filter(req => req !== name)
        dispatch(setOptionIncompatibilities({optionId: oId, incompatibilities}))
    }
    
    dispatch(removeOption({groupId, optionId: name}))
}

export const createModel = (modelName, description, options = []) => (dispatch) => {
    dispatch(addModel({
        modelName,
        options,
        description
    }))
}
export const createDefaultModel = (modelName) => (dispatch) => {
    dispatch(setDefaultModel(modelName))
}
export const changeModelOptions = (modelId, options) => (dispatch) => {
    dispatch(setModelOptions({
        modelId,
        options
    }))
}

export const editConfiguration = (configId) => async (dispatch, getState) => {
    // reset builder
    dispatch(resetBuild)

    // get configuration for every language
    Object.values(languageNames).forEach(lang => {
        setAcceptLanguage(lang)
        
        fetchId(configId)
        .then(config => {
            console.log('loaded config from language', lang)
            console.log(config)
            const c = {
                ...config,
                // name: '',
                // description: '',
                // images: [],
                // rules: {}, 
                configId,
                options: config.options.map(o => {
                    return {
                        ...o,
                        // id: o.id,
                        // productNumber: o.productNumber,
                        groupId: getBuilderGroupIdByOptionId(getState(), o.id)
                    }
                }),
                optionSections: config.optionSections.map(s => {
                    return {
                        ...s
                        // id: s.id, optionGroupIds: s.optionGroupIds
                    }
                }),
                optionGroups: config.optionGroups.map(g => {
                    return {
                        ...g,
                        replacement: Object.keys(config.rules.replacementGroups).find(gId => gId === g.id) ? true : false
                    }
                }),
                language: lang
            }
            dispatch(setConfigurationToEdit(c))
        })
        .catch(err => {
            console.log(`Could not get configuration ${configId} with language ${lang}`)
            console.log(err)
        })

    })
}

export const finishConfigurationBuild = () => async (dispatch, getState) => {
    dispatch(loadingStarted())

    let configuration = selectBuilderConfiguration(getState())
    
    writeToLocalStorage(initialConfiguration, 'builder')

    // call put configuration (not post new one), when there is a config id set
    if (configuration.configId) {
        putConfiguration(configuration)
        .then(res => {
            dispatch(loadingSucceeded(res))
        })
        .catch(err => {
            dispatch(loadingFailed(err))
        })
        return
    }

    postConfiguration(configuration)
    .then(res => {
        dispatch(loadingSucceeded(res))
    })
    .catch(error => {
        dispatch(loadingFailed(error))
    })
}



// Action creators are generated for each case reducer function
export const { 
    setImages, setAvailableImages,
    addSection, changeSectionProperties, removeSection,
    addOptionGroup, changeGroupProperties, setGroupRequirements, setGroupIsRequired, setGroupIsReplacement, removeOptionGroup,
    addOption, changeOptionProperties, setOptionPrice, setOptionRequirements, setOptionIncompatibilities, removeOption,
    addModel, changeModelProperties, setDefaultModel, setModelOptions, removeModel,
    setBasePrice, setDescription, setName, changeInputLanguage,
    resetBuild, setConfigurationToEdit,
    loadingStarted, loadingSucceeded, loadingFailed, loadingHandled
} = builderSlice.actions

export default builderSlice.reducer