class MudEnhancedChartHelper {

    constructor() {
    }

    triggerAnimation(chartid) {
        var elements = document.querySelectorAll('[data-chartid="' + chartid + '"]')

        for (var i = 0; i < elements.length; i++) {
            elements[i].firstChild.beginElement();
        }
    }

};

window.mudEnhancedChartHelper = new MudEnhancedChartHelper();