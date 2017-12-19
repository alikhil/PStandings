import * as React from 'react';
import { Link, NavLink } from 'react-router-dom';
import { RouteComponentProps } from 'react-router';

interface Contest {
    name: string;
    id: string;

}

interface NavMenuState {
    contests: Contest[];
    loading: boolean;
    generation: string;
    _isUnmounted: boolean;
}

export class NavMenu extends React.Component<{ generation: string }, NavMenuState> {
    constructor() {
        super();
        this.state = { contests: [], loading: true, generation: '', _isUnmounted: false };
    }

    private getData() {
        fetch(`api/pcms/${this.props.generation}/contests`)
        
                    .then(response => { 
                        return response.json() as Promise<Contest[]>; 
                    })
                    .then(data => {
                        if (!this.state._isUnmounted) {
                            this.setState({ contests: data, loading: false, generation: this.props.generation });
                        }
                });
    }
    
    public componentWillUnmount() {
        this.setState({...this.state, _isUnmounted: true });
    }

    public componentDidMount() {
        this.getData();
    }

    // public shouldComponentUpdate(nextProps: { generation:string }, nexState: NavMenuState) {
    //     console.log(nextProps);
    //     console.log(nexState);
    //     console.log(nextProps.generation !== this.state.generation);
    //     return (nextProps.generation !== this.state.generation);
    // }

    public render() {
        const content = 
            this.state.loading 
                ? <li><a><span className='glyphicon glyphicon-refresh glyphicon-refresh-animate'></span>Loading</a></li>
                : NavMenu.renderContests(this.state.contests, this.props.generation);

        return <div className='main-nav'>
                <div className='navbar navbar-inverse scrollable-navbar'>
                <div className='navbar-header'>
                    <button type='button' className='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>
                        <span className='sr-only'>Toggle navigation</span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                        <span className='icon-bar'></span>
                    </button>
                    <Link className='navbar-brand' to={ `/${this.props.generation}/` }>PStandings</Link>
                </div>
                <div className='clearfix'></div>
                <div className='navbar-collapse collapse'>
                    <ul className='nav navbar-nav'>
                        <li>
                            <NavLink to={ `/${this.props.generation}/` } exact activeClassName='active'>
                                <span className='glyphicon glyphicon-flag'></span> Итоговая таблица
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={ `/${this.props.generation}/analytics` } exact activeClassName='active'>
                                <span className='glyphicon glyphicon-search'></span> Аналитика
                            </NavLink>
                        </li>
                        <li>
                            <NavLink to={ `/${this.props.generation}/submissions` } activeClassName='active'>
                                <span className='glyphicon glyphicon-send'></span> Посылки
                            </NavLink>
                        </li>
                        { content }
                        
                    </ul>
                </div>
            </div>
        </div>;
    }

    private static renderContests(contests: Contest[], generation: string) {
        return contests.map(contest =>
            <li key={contest.id.toString()}>
                <NavLink to={`/${generation}/contests/${contest.id}`} exact activeClassName='active'>
                    <span className='glyphicon glyphicon-list-alt'></span> {contest.name}
                </NavLink>
            </li>
        )
    }
}
