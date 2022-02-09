import { createSlice } from '@reduxjs/toolkit'
import { postConfiguration } from '../../api/configurationAPI'
import { readFromLocalStorage, writeToLocalStorage } from '../../App'
import { defaultLang } from '../../lang'
import { extractGroupsFromBuilderSection, extractModelNameFromBuilderModel, extractModelOptionsFromBuilderModel, extractOptionsFromBuilderGroup, getBuilderGroupById, getBuilderSectionById, getDoesGroupdExist, getDoesOptionExist, getDoesSectionExist, selectBuilderGroupRequirements, selectBuilderModels, selectBuilderOptionIncompatibilities, selectBuilderOptionRequirements, selectBuilderConfiguration } from './builderSelectors'


const initialState = {
    configuration: {
        images: [],
        options: [
            {
                id: 'ALLOY19',
                groupId: 'WHEELS'
            },
            {
                id: 'STEEL16',
                groupId: 'WHEELS'
            },
            {
                id: 'RED',
                groupId: 'COLOR_GROUP'
            },
            {
                id: 'BLUE',
                groupId: 'COLOR_GROUP'
            }
        ],
        optionSections: [
            {
                id: 'EXTERIOR',
                optionGroupIds: [
                    'WHEELS', 'COLOR_GROUP'
                ]
            }
        ],
        optionGroups: [
            {
                id: 'WHEELS',
                optionIds: [
                    'ALLOY19', 'STEEL16'
                ],
                required: true,
                replacement: true
            },
            {
                id: 'COLOR_GROUP',
                optionIds: [
                    'BLUE', 'RED'
                ],
                required: false,
                replacement: false
            }
        ],
        rules: {
            basePrice: 500,
            // defaultOptions: [/*BLUE*/],
            defaultModel: '',
            models: [
                {
                    name: 'Sport',
                    options: ['ALLOY19', 'RED'],
                },
                {
                    name: 'Basic',
                    options: ['STEEL16', 'BLUE'],
                }
            ],
            // replacementGroups: {
            //     // COLOR_GROUP: [
            //     //     'BLUE'
            //     // ]
            // },
            groupRequirements: {
                // PANORAMATYPE_GROUP: ['PANORAMA_GROUP']
                COLOR_GROUP: ['WHEELS'],
                WHEELS: ['COLOR_GROUP']
            },
            requirements: {
                BLUE: ['STEEL16'],
                ALLOY19: ['RED']
            },
            incompatibilities: {
                // PANORAMAROOF: ['PETROL']
                BLUE: ['ALLOY19'],
                STEEL16: ['RED']
            },
            priceList: {
                'BLUE': 200
            }
        },
        languages: {
            en: {
                name: '',
                description: '',
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
                        id: 'BLUE',
                        name: 'blue',
                        description: 'Farbe blau',
                    }
                ],
                optionSections: [
                    {
                        id: 'EXTERIOR',
                        name: 'Exterior'
                    }
                ],
                optionGroups: [
                    {
                        id: 'WHEELS',
                        name: 'Wheels',
                        description: 'round stuff'
                    },
                    {
                        id: 'COLOR_GROUP',
                        name: 'Color',
                        description: 'of the car',
                    }
                ],
                models: [
                    {
                        modelNameId: 'Sport',
                        name: 'Sport',
                        description: 'description, description, description, description, description, description, description, description, description, description, description, description, description, description, description, description,'
                    },
                    {
                        modelNameId: 'Basic',
                        name: 'Basic',
                        description: 'description, description, description, description, description, description, description, description, '
                    }
                ]
            },
            de: {
                name: '',
                description: '',
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
                        id: 'BLUE',
                        name: 'blau',
                        description: 'Farbe blau',
                    }
                ],
                optionSections: [
                    {
                        id: 'EXTERIOR',
                        name: 'Außen'
                    }
                ],
                optionGroups: [
                    {
                        id: 'WHEELS',
                        name: 'Reifen',
                        description: 'Reifen halt'
                    },
                    {
                        id: 'COLOR_GROUP',
                        name: 'Farbe',
                        description: 'Farbe vom Auto'
                    }
                ],
                models: [
                    {
                        modelNameId: 'Sport',
                        name: 'Sport',
                        description: 'Beschreibung Sport'
                    },
                    {
                        modelNameId: 'Basic',
                        name: 'Standard',
                        description: 'Beschreibung Standard'
                    }
                ]
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
    },
    currentLanguage: defaultLang,
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const builderSlice = createSlice({
    name: 'builder',
    initialState: initialState,
    reducers: {
        addSection: (state, action) => {
            const { name, language } = action.payload

            // add to sections
            state.configuration.optionSections.push({
                id: name,
                // name: name,
                optionGroupIds: []
            })

            // add to language
            const sections = state.configuration.languages[language].optionSections
            const section = sections.find(s => s.id === name)
            if (section) section.name = name
            else sections.push({id: name, name: name})

            // state.configuration.languages[language].optionSections[name] = {name: name}
        },
        changeSectionProperties: (state, action) => {
            // TODO: call with language
            const { sectionId, newName, language } = action.payload

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
            for (const langObj of state.configuration.languages) {
                langObj.optionSections = langObj.optionSections.filter(s => s.id !== sectionId)
            }
        },
        addOptionGroup: (state, action) => {
            // TODO: call with language
            const { sectionId, groupId, name, description, isRequired, isReplacementGroup, language } = action.payload

            state.configuration.optionGroups.push({
                id: groupId,
                // name: name,
                // description: description,
                required: isRequired,
                replacement: isReplacementGroup,
                optionIds: []
            })

            // add to language
            const groups = state.configuration.languages[state.currentLanguage].optionGroups
            let group = groups.find(g => g.id === groupId)
            const updatedGroup = {
                id: groupId, 
                name, 
                description
            }
            if (group) group = updatedGroup
            else groups.push(updatedGroup)

            // add to section
            const section = state.configuration.optionSections.find(s => s.id === sectionId)
            if (section) section.optionGroupIds.push(groupId)
        },
        changeGroupProperties: (state, action) => {
            // TODO: call with language
            const { groupId, newName, newDescription, language } = action.payload

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
            for (const langObj of state.configuration.languages) {
                langObj.optionGroups = langObj.optionGroups.filter(g => g.id !== groupId)
            }

            // remove group from section
            const section = state.configuration.optionSections.find(s => s.id === sectionId)
            if (section) section.optionGroupIds = section.optionGroupIds.filter(g => g !== groupId)
        },
        addOption: (state, action) => {
            // TODO: call with language
            const { groupId, optionId, name, description, price, language } = action.payload

            // add option to options list
            state.configuration.options.push({
                id: optionId,
                // name: name,
                // description: description,
                groupId: groupId
            })

            // add to language
            const options = state.configuration.languages[language].options
            let option = options.find(o => o.id === optionId)
            const updatedOption = {
                id: optionId,
                name,
                description
            }
            if (option) option = updatedOption
            else options.push(updatedOption)

            // add option to group
            const group = state.configuration.optionGroups.find(g => g.id === groupId)
            if (group) group.optionIds.push(optionId)

            // add option price to pricelist in rules
            if (price) state.configuration.rules.priceList[optionId] = price
        },
        changeOptionProperties: (state, action) => {
            // TODO: call with language
            const { optionId, newName, newDescription, language } = action.payload

            const option = state.configuration.languages[state.currentLanguage].options.find(o => o.id === optionId)
            if (option) {
                option.name = newName || option.name
                option.description = newDescription || option.description
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
            for (const langObj of state.configuration.languages) {
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
            // TODO: call with language
            const { modelName, options, description, language } = action.payload

            state.configuration.rules.models.push({
                name: modelName, 
                options, 
                description
            })

            // add to language
            const models = state.configuration.languages[state.currentLanguage].models
            let model = models.find(m => m.name === modelName)
            const updatedModel = {
                modelNameId: modelName,
                name: modelName,
                description
            }
            if (model) model = updatedModel
            else models.push(updatedModel)
        },
        changeModelProperties: (state, action) => {
            // TODO: call with language
            const { modelName, newName, newDescription, language } = action.payload

            const model = state.configuration.languages[state.currentLanguage].models.find(m => m.modelNameId === modelName)
            if (model) {
                model.name = newName || model.name
                model.description = newDescription || model.description
            }
        },
        setDefaultModel: (state, action) => {
            state.configuration.rules.defaultModel = action.payload
        },
        setModelOptions: (state, action) => {
            const { modelName, options } = action.payload
            
            const model = state.configuration.rules.models.find(m => m.name === modelName)
            if (model) model.options = options
        },
        removeModel: (state, action) => {
            const { modelName } = action.payload

            // remove from models
            state.configuration.rules.models = state.configuration.rules.models.filter(m => m.name !== modelName)

            // remove from languages
            for (const langObj of state.configuration.languages) {
                langObj.models = langObj.models.filter(m => m.name !== modelName)
            }
        },
        setBasePrice: (state, action) => {
            state.configuration.rules.basePrice = action.payload
        },
        setDescription: (state, action) => {
            // TODO: call with language
            const description = action.payload

            state.configuration.languages[state.currentLanguage].description = description
        },
        setName: (state, action) => {
            // TODO: call with language
            const { name, language } = action.payload

            state.configuration.languages[state.currentLanguage].name = name
        },
        resetBuild: (state, action) => {
            state.configuration = initialState
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

export const saveBuilderToStorage = () => (dispatch, getState) => {
    // writeToLocalStorage(selectBuilderConfiguration(getState()), 'builder')
}

export const createSection = (sectionName, language = defaultLang) => (dispatch, getState) => {
    // check if section doesn't already exist
    const sectionExists = getDoesSectionExist(getState(), sectionName)
    if (sectionExists) {
        return false
    }

    dispatch(addSection({name: sectionName, language}))
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

export const createOption = (groupId, name, description, price = 0) => (dispatch, getState) => {
    const optionId = `${name}_${groupId}`.replace(' ', '_')

    // check if option doesn't already exist
    const optionExists = getDoesOptionExist(getState(), optionId)
    if (optionExists) {
        return false
    }

    dispatch(addOption({groupId, optionId, name, description, price}))
    return true
}
export const deleteOption = (groupId, name) => (dispatch, getState) => {
    // remove option from models
    const models = selectBuilderModels(getState())
    models.forEach(model => {
        const modelName = extractModelNameFromBuilderModel(model)
        const newOptions = extractModelOptionsFromBuilderModel(model).filter(optionId => optionId !== name)
        dispatch(changeModelOptions(modelName, newOptions))
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

export const createModel = (modelName, options, description) => (dispatch) => {
    dispatch(addModel({
        modelName,
        options,
        description
    }))
}
export const createDefaultModel = (modelName) => (dispatch) => {
    dispatch(setDefaultModel(modelName))
}
export const changeModelOptions = (modelName, options) => (dispatch) => {
    dispatch(setModelOptions({
        modelName,
        options
    }))
}

export const finishConfigurationBuild = (name = '') => async (dispatch, getState) => {
    dispatch(loadingStarted())

    let configuration = selectBuilderConfiguration(getState())
    
    if (name) {
        dispatch(setName(name))
    }

    writeToLocalStorage(initialState, 'builder')

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
    addSection, changeSectionProperties, removeSection, 
    addOptionGroup, changeGroupProperties, setGroupRequirements, setGroupIsRequired, setGroupIsReplacement, removeOptionGroup, 
    addOption, changeOptionProperties, setOptionPrice, setOptionRequirements, setOptionIncompatibilities, removeOption, 
    addModel, changeModelProperties, setDefaultModel, setModelOptions, removeModel,
    setBasePrice, setDescription, setName, 
    resetBuild, 
    loadingStarted, loadingSucceeded, loadingFailed, loadingHandled 
} = builderSlice.actions

export default builderSlice.reducer