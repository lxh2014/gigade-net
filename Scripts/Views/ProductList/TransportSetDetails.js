var transportCheck = {
    xtype: 'checkboxgroup',
    fieldLabel: LOGISTICS_DISTRMODE,
    id: 'transportCheck',
    columns: 2,
    readOnly: true,
    vertical: true,
    items: [
        { boxLabel: DISTRMODE_1, inputValue: '1|11', name: 'transportGroup' },
        { boxLabel: DISTRMODE_2, inputValue: '2|21', name: 'transportGroup' },
        { boxLabel: DISTRMODE_3, inputValue: '3|31', name: 'transportGroup' },
        { boxLabel: DISTRMODE_4, inputValue: '2|22', name: 'transportGroup' },
        { boxLabel: DISTRMODE_5, inputValue: '1|12', name: 'transportGroup' }
    ]
}