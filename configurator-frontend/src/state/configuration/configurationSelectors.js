import { createSelector } from 'reselect'

export const GROUP_ERRORS = {
    atLeastOne: 'groupErrorAtLeastOne' // the name is the key in the language file 
}

export const OPTION_ERRORS = {
    requirementsNotMet: 'optionRequirementsNotMet',
    hasIncompatibilities: 'optionHasIncompatibilities'
}

// input selectors:
const selectGroupId = (state, groupId) =>               groupId
const selectOptionId = (state, optionId) =>             optionId
const selectModelName = (state, modelName) =>           modelName
const selectSectionId = (state, sectionId) =>           sectionId

const selectAllOptionIncompatibilities = state =>       state.configuration.configuration.rules.incompatibilities
const selectAllOptionRequirements = state =>            state.configuration.configuration.rules.requirements
const selectReplacementGroups = state =>                state.configuration.configuration.rules.replacementGroups

const selectAllGroupRequirements = state =>             state.configuration.configuration.rules.groupRequirements

export const selectConfigurationStatus = state =>       state.configuration.status

export const selectConfigurationId = (state) =>         state.configuration.configuration.configId

export const selectBasePrice = state =>                 state.configuration.configuration.rules?.basePrice || 0
export const selectPriceList = state =>                 state.configuration.configuration.rules?.priceList || []
// export const selectDefaultOptions = state =>            state.configuration.configuration.rules?.defaultOptions || []
export const selectDefaultModel = state =>              state.configuration.configuration.rules?.defaultModel || ''
export const selectSelectedModel = state =>             state.configuration.selectedModel
export const selectModels = state =>                    state.configuration.configuration.rules?.models || []
export const selectDefaultOptions = state =>            state.configuration.configuration.rules?.defaultOptions || []
export const selectConfigurationName = state =>         state.configuration.configuration.name
export const selectConfigurationDescription = state =>  state.configuration.configuration.description

export const selectOptions = state =>                   state.configuration.configuration.options
export const selectOptionGroups = state =>              state.configuration.configuration.optionGroups
export const selectOptionSections = state =>            state.configuration.configuration.optionSections
export const selectSelectedOptions = state =>           state.configuration.selectedOptions


// export const getCurrentModel = createSelector([selectModels, selectSelectedModel, selectDefaultModel], (models, selectedModel, defaultModel) => {
//     const modelName = selectedModel ? selectedModel : defaultModel

//     return models.find(m => m.name === modelName)
// })
export const getModelOptions = createSelector([selectModels, selectModelName], (models, modelName) => {
    const model = models.find(m => m.modelName === modelName)

    return model ? model.options : []
})

export const getOptionsInSection = createSelector([selectSectionId, selectOptionGroups, selectOptionSections], (sectionId, groups, sections) => {
    
    // find the specific section
    const section = sections.find(s => s.id === sectionId)
    
    // get all groups in that section
    const groupsInSection = groups.filter(g => section.optionGroupIds.includes(g.id))

    // get all options from these groups
    let options = []
    groupsInSection.forEach(g => {
        options.push(...g.optionIds)
    })

    return options
})

export const getOption = createSelector([selectOptions, selectOptionId], (options, id) => {
    // console.log('Option output')
    // console.log('---> ' + id)
    return options.find(o => o.id === id)
})
export const getOptionName = createSelector([getOption, selectOptionId], (option, id) => {
    return option.name
})


export const getIsOptionSelected = createSelector([selectSelectedOptions, selectOptionId], (selectedOptions, id) => {
    // console.log('Is option selected output')
    return selectedOptions.includes(id)
})

// all options that are in the same replacement group (or null if the option is in no replacement group)
export const getOptionReplacementGroup = createSelector([selectReplacementGroups, selectOptionId], (replacementGroups, id) => {
    // console.log('Option replacement groups output')

    for (let groupName of Object.keys(replacementGroups)) {
        const replacementGroup = replacementGroups[groupName]
        if (replacementGroup.includes(id)) {
            return replacementGroup
        }
    }

    return null
})

// -- getIsOptionSelectable -->
const getOptionIncompatibilities = createSelector([selectAllOptionIncompatibilities, selectOptionId], (incompatibilities, optionId) => {
    // console.log('Option Incompatitbilities output')
    return incompatibilities[optionId]
})
const getIsOptionCompatible = createSelector([selectSelectedOptions, getOptionIncompatibilities], (selectedOptions, incompatibilities) => {
    // returns an array:
    //  -> 1. index: if the option is compatible or not
    //  -> 2. index: options that are incompatible

    if (!incompatibilities) return [true, null]

    let compatible = true
    let incompatibleOptions = []
    incompatibilities.forEach(incompatibility => {
        // if there is an incompatibility in the selected options, the option is not compatible
        if (selectedOptions.includes(incompatibility)) {
            compatible = false
            incompatibleOptions.push(incompatibility)
        }
    })
    return [compatible, incompatibleOptions]
})
const getOptionRequirements = createSelector([selectAllOptionRequirements, selectOptionId], (requirements, optionId) => {
    // console.log('Option Requirements output')
    return requirements[optionId]
})
const getIsOptionRequirementsMet = createSelector([selectSelectedOptions, getOptionRequirements], (selectedOptions, requirements) => {
    // returns an array:
    //  -> 1. index: if the options requirements are met or not (required options are selected)
    //  -> 2. index: options that are required for this option

    // console.log('Is option requirements met output')

    if (!requirements) return [true, null] // if there are no requirements, return true
    
    let reqMet = true
    let requiredOptions = []
    requirements.forEach(requirement => {
        // if there is a requirement not in the selected options, the option does not meet its requirements
        if (!selectedOptions.includes(requirement)) {
            reqMet = false
            requiredOptions.push(requirement)
        }
    })
    return [reqMet, requiredOptions]
})
// returns null ()
export const getIsOptionSelectable = createSelector([getIsOptionRequirementsMet, getIsOptionCompatible], (isRequirementsMet, isCompatible) => {
    // console.log('Is option selectable output')

    if (!isRequirementsMet[0])      return [OPTION_ERRORS.requirementsNotMet, isRequirementsMet[1]]//.join(', ') + (isRequirementsMet[1].length > 1 ? ' are ' : ' is ') + 'required!']
    if (!isCompatible[0])           return [OPTION_ERRORS.hasIncompatibilities, isCompatible[1]]//'Not compatible with: ' + isCompatible[1].join(', ')

    return null
})
// <-- getIsOptionSelectable --

// -- group logic -->
const getGroupRequirements = createSelector([selectAllGroupRequirements, selectGroupId], (allRequirements, groupId) => {
    const groupRequirements = allRequirements[groupId]
    if (!groupRequirements) return []
    return groupRequirements
})
const getGroup = createSelector([selectOptionGroups, selectGroupId], (groups, groupId) => {
    const selectedGroup = groups.find(g => g.id === groupId)
    if (!selectedGroup) {
        console.log('Can not get is group valid because there is no group with the id')
        return null
    }
    return selectedGroup
})
// returns null (no error) or the specific error from 'GROUP_ERRORS'
export const getIsGroupValid = createSelector([getGroup, getGroupRequirements, selectOptionGroups, selectSelectedOptions], (selectedGroup, requirements, groups, selectedOptions) => {

    // if the group is not required it is valid
    if (!selectedGroup.required) return null

    // if the group is required, but one of the required groups is not yet selected, it is also valid
    const requiredGroups = groups.filter(g => requirements.includes(g.id))
    let requirementsMet = true
    requiredGroups.forEach(requirement => {
        let groupSelected = false
        requirement.optionIds.forEach(option => {
            if (selectedOptions.includes(option)) {
                groupSelected = true
            }
        })
        // if just one group that is required for this group has not selected any options -> the requirements are not met
        if (!groupSelected) requirementsMet = false
    })
    // the requirements are not yet selected -> this group is valid (cant select if the requirement is not selected)
    if (!requirementsMet) return null

    // the group is required and there is no option selected -> error at least has to be selected
    let atLeastOneOptionSelected = false
    selectedGroup.optionIds.forEach(option => {
        if (selectedOptions.includes(option)) {
            atLeastOneOptionSelected = true
        }
    })
    if (!atLeastOneOptionSelected) {
        return GROUP_ERRORS.atLeastOne
    }

    return null
})
// <-- group logic --

export const getOptionPrice = createSelector([selectPriceList, selectOptionId], (priceList, id) => {
    // console.log('Option price output')
    return priceList[id]
})

// the total price of all selected options plus the base price
export const getCurrentPrice = createSelector([selectSelectedOptions, selectBasePrice, selectPriceList], (selectedOptions, basePrice, priceList) => {
    // console.log('Current Price output')

    const price = selectedOptions.reduce((total, optionId) => {
        if (priceList[optionId]) {
            return total + priceList[optionId]
        } else {
            return total
        }
    }, basePrice)

    return price
})

// the options that cant be used if this option is deselected
export const getDependentOptionsDeselect = createSelector(
    [selectSelectedOptions, selectOptionId, selectAllOptionRequirements], 
    (selectedOptions, selectedOptionId, requirements) => {

    let dependencies = dependenciesFromDependencyLists(selectedOptionId, requirements)

    // only get the dependencies from the selected options
    dependencies = dependencies.filter(d => selectedOptions.includes(d))

    return dependencies
})
// the options that cant be used if this option selected
export const getDependentOptionsSelect = createSelector(
    [selectSelectedOptions, selectOptionId, selectAllOptionIncompatibilities], 
    (selectedOptions, selectedOptionId, incompatibilities) => {

    let dependencies = dependenciesFromDependencyLists(selectedOptionId, incompatibilities)
    
    // only get the dependencies from the selected options
    dependencies = dependencies.filter(d => selectedOptions.includes(d))

    return dependencies
})
function dependenciesFromDependencyLists(dependentOption, ...dependencyLists) {
    let dependencies = []

    // for every dependency list
    for (const dependencyList of dependencyLists) {

        // for every dependency in the dependency list
        for (const dependency in dependencyList) {
            // check if the dependent option is included in the dependencies
            if (dependencyList[dependency].includes(dependentOption)) {
                // add the dependency (the option that depends on the dependentOption)
                dependencies.push(dependency)
            }
        }

    }

    return dependencies
}