/**
 * Increments visits count on admin site.
 */
export function incrementVisitsCount () {
    let visitsCount = localStorage.getItem('visitsCount') ?? 0;
    visitsCount++;
    localStorage.setItem('visitsCount', visitsCount);

    const visits = document.getElementById('visits');
    visits.textContent = visitsCount;
}
