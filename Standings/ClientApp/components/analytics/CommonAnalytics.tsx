import autobind from 'autobind-decorator';
import * as mm from 'moment';
import * as React from 'react';
import { RouteComponentProps } from 'react-router-dom';
const withQuery = require('with-query') as (url: string, ob: any) => string;

import { DatePicker } from '../DateRangePicker';
import { ComponentWithNav } from '../ComponentWithNav';
import { NavMenu } from '../NavMenu';
import { DateRanges } from '../Submissions';
import { Line, Bar } from 'react-chartjs-2';
import { ButtonGroup, Button, MenuItem, DropdownButton } from 'react-bootstrap';


enum StatType {
    month = 'month',
    day = 'day'
}

interface CommonAnaliticsState extends DateRanges {
    isLoading: boolean;
    prefix: string;
    type: StatType;
    stat: Stat;
}


interface Stat {
    type: StatType;
    keys: string[];
    all: number[];
    success: number[];
}

export class CommonAnalyticsComponent extends React.Component<{ generation: string }, CommonAnaliticsState> {
    
    constructor() {
        super();
        this.state = { 
            isLoading: true,
            prefix: "",
            startDate: new Date(2016, 9, 1),
            endDate: mm().toDate(),
            type: StatType.month,
            stat: { type: StatType.month, all: [], success: [], keys: [] },
        };
    }

    @autobind
    private onApply(event: string, picker: DateRanges) {
        this.setState({...this.state, startDate: mm(picker.startDate).toDate(), endDate: mm(picker.endDate).toDate() });
    }

    public componentDidMount() {
        this.loadDataset();
    }

    @autobind
    private loadDataset() {
        console.log("load dataset called");
        fetch(withQuery(`api/pcms/${this.props.generation}/analytics/common`, {
            type: this.state.type,
            prefix: this.state.prefix,
            startDate: mm(this.state.startDate).unix(),
            endDate: mm(this.state.endDate).unix()
        }))
            .then(response => response.json() as Promise<Stat>)
            .then(data => this.setState({ ...this.state, stat: data, isLoading: false }));
    }

    @autobind
    private onFilterClick() {
        this.loadDataset();
    }

    @autobind
    private updatePrefix(event: React.FormEvent<HTMLInputElement>) {
        this.setState({...this.state, prefix: event.currentTarget.value });
    }


    @autobind
    private getTitleByType(type: StatType) {
        switch (type) {
            case StatType.day:
                return 'По дням';
            case StatType.month:
                return 'По месяцам';
            default:
                const exhaustiveCheck: never = type;
        }
    }

    @autobind
    private onSelectType(key: any) {
        const type = key as StatType;
        this.setState({...this.state, type });
    }

    public render() {
        const chartOptions = { };

        const chartData = {
            labels:  this.state.isLoading ? [] : this.state.stat.keys,
            datasets: [{
                label: '# Успешных посылок',
                data: this.state.isLoading ? [] : this.state.stat.success,
                backgroundColor: 'rgba(30, 181, 120, 0.2)',
                borderColor: 'rgba(30, 181, 120, 1)',
                borderWidth: 1
            },
            {
                label: "# всего попыток",
                data: this.state.isLoading ? [] : this.state.stat.all,
                backgroundColor:'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        };

        const Chart = this.state.type === StatType.day ? Line : Bar;
        return <div> 
            
            <div>
                <form className='form-inline'>
                    <div className='form-group mr-10'>
                        <label htmlFor='groupInput' className='mr-10' >Группа</label>
                        <input type='text' className='form-control' id='groupInput' placeholder='гр1' onChange={this.updatePrefix}/>
                    </div>
                    <div className='form-group mr-10'>
                        <DatePicker 
                            startDate={mm(this.state.startDate)}
                            endDate={mm(this.state.endDate)} 
                            onApply={this.onApply}/>
                    </div>
                    <DropdownButton className='mr-10' title={this.getTitleByType(this.state.type)} id='bg-nested-dropdown'>
                        <MenuItem onSelect={this.onSelectType} eventKey='day'>По дням</MenuItem>
                        <MenuItem onSelect={this.onSelectType} eventKey='month'>По месяцам</MenuItem>
                    </DropdownButton>
                    <Button className='btn btn-default' onClick={this.onFilterClick}>Отфильтровать</Button>
                </form>
            </div>
            <br/>
            { this.state.isLoading 
                ? <p>Loading...</p>
                : <Chart data={chartData} options={chartOptions} width={600} height={250}/>
            }
        </div>;
    }
}