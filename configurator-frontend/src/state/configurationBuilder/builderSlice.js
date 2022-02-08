import { createSlice } from '@reduxjs/toolkit'
import { postConfiguration } from '../../api/configurationAPI'
import { selectConfigurationName } from '../configuration/configurationSelectors'
import { extractGroupsFromBuilderSection, extractModelNameFromBuilderModel, extractModelOptionsFromBuilderModel, extractOptionsFromBuilderGroup, getBuilderGroupById, getBuilderSectionById, getDoesGroupdExist, getDoesOptionExist, getDoesSectionExist, selectBuilderGroupRequirements, selectBuilderModels, selectBuilderOptionIncompatibilities, selectBuilderOptionRequirements, selectConfiguration } from './builderSelectors'


const initialState = {
    configuration: {
        name: '',
        description: '',
        images: [],
        options: [
            {
                id: 'ALLOY19',
                name: '19 inch Alloy',
                description: 'description',
                groupId: 'WHEELS'
            },
            {
                id: 'STEEL16',
                name: '16 inch Steel',
                description: 'description',
                groupId: 'WHEELS'
            },
            {
                id: 'RED',
                name: 'red',
                description: 'color',
                groupId: 'COLOR_GROUP'
            },
            {
                id: 'BLUE',
                name: 'blue',
                description: 'color',
                groupId: 'COLOR_GROUP'
            }
        ],
        optionSections: [
            {
                id: 'EXTERIOR',
                name: 'Exterior',
                optionGroupIds: [
                    'WHEELS', 'COLOR_GROUP'
                ]
            }
        ],
        optionGroups: [
            {
                id: 'WHEELS',
                name: 'Wheels',
                description: 'round objects for diriving',
                optionIds: [
                    'ALLOY19', 'STEEL16'
                ],
                required: true,
                replacement: true
            },
            {
                id: 'COLOR_GROUP',
                name: 'Color',
                description: 'of the car',
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
                    description: 'description, description, description, description, description, description, description, description, description, description, description, description, description, description, description, description,'
                },
                {
                    name: 'Basic',
                    options: ['STEEL16', 'BLUE'],
                    description: 'description, description, description, description, description, description, description, description, '
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
        }
    },
    status: 'idle', // | 'loading' | 'succeeded' | 'failed'
    error: null
}

export const builderSlice = createSlice({
    name: 'builder',
    initialState,
    reducers: {
        addSection: (state, action) => {
            state.configuration.optionSections.push({
                id: action.payload,
                name: action.payload,
                optionGroupIds: []
            })
        },
        removeSection: (state, action) => {
            const sectionId = action.payload

            state.configuration.optionSections = state.configuration.optionSections.filter(s => s.id !== sectionId)
        },
        addOptionGroup: (state, action) => {
            const { sectionId, groupId, name, description, isRequired, isReplacementGroup } = action.payload

            state.configuration.optionGroups.push({
                id: groupId,
                name: name,
                description: description,
                required: isRequired,
                replacement: isReplacementGroup,
                optionIds: []
            })

            const section = state.configuration.optionSections.find(s => s.id === sectionId)
            if (section) section.optionGroupIds.push(groupId)
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
        removeOptionGroup: (state, action) => {
            const { groupId, sectionId } = action.payload

            // remove group requirements
            if (state.configuration.rules.groupRequirements[groupId]) delete state.configuration.rules.groupRequirements[groupId]

            // remove group from group list
            state.configuration.optionGroups = state.configuration.optionGroups.filter(g => g.id !== groupId)

            // remove group from section
            const section = state.configuration.optionSections.find(s => s.id === sectionId)
            if (section) section.optionGroupIds = section.optionGroupIds.filter(g => g !== groupId)
        },
        addOption: (state, action) => {
            const { groupId, optionId, name, description, price } = action.payload

            // add option to options list
            state.configuration.options.push({
                id: optionId,
                name: name,
                description: description,
                groupId: groupId
            })

            // add option to group
            const group = state.configuration.optionGroups.find(g => g.id === groupId)
            if (group) group.optionIds.push(optionId)

            // add option price to pricelist in rules
            if (price) state.configuration.rules.priceList[optionId] = price
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
                name: modelName, 
                options, 
                description
            })
        },
        removeModel: (state, action) => {
            state.configuration.rules.models = state.configuration.rules.models.filter(model => model.name !== action.payload)
        },
        setDefaultModel: (state, action) => {
            state.configuration.rules.defaultModel = action.payload
        },
        setModelOptions: (state, action) => {
            const { modelName, options } = action.payload
            
            const model = state.configuration.rules.models.find(m => m.name === modelName)
            if (model) model.options = options
        },
        setBasePrice: (state, action) => {
            state.configuration.rules.basePrice = action.payload
        },
        setDescription: (state, action) => {
            state.configuration.description = action.payload
        },
        setName: (state, action) => {
            state.configuration.name = action.payload
        },
        resetBuild: (state, action) => {
            state.configuration = {}
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
    extraReducers: (builder) => {
        // builder.addCase(actionFromOtherSlice, (state, action) => {
        //     state.status = 'succeeded'
        // })
    }
})


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

    let configuration = selectConfiguration(getState())
    
    if (name) {
        dispatch(setName(name))
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
export const { addSection, removeSection, addOptionGroup, setGroupRequirements, removeOptionGroup, addOption, setOptionPrice, setOptionRequirements, setOptionIncompatibilities, removeOption, addModel, removeModel, setDefaultModel, setModelOptions, setBasePrice, setDescription, setName, resetBuild, loadingStarted, loadingSucceeded, loadingFailed, loadingHandled } = builderSlice.actions

export default builderSlice.reducer