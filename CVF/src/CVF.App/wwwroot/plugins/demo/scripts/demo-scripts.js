(function ($enums) {

    var demoTypeEnum = [{
        name: 'type1',
        displayName: '类型1',
        value: 0
    }, {
        name: 'type2',
        displayName: '类型2',
        value: 1
    }, {
        name: 'type3',
        displayName: '类型3',
        value: 2
    }];

    $enums.items.push({ name: 'demoTypeEnum', items: demoTypeEnum });
})($enums || ($enums = {}));