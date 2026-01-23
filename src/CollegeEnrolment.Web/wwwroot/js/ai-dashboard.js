(async function () {
  async function getJson(url) {
    const res = await fetch(url);
    if (!res.ok) throw new Error(`HTTP ${res.status} for ${url}`);
    return await res.json();
  }

  function groupBy(arr, keyFn) {
    const map = new Map();
    for (const item of arr) {
      const key = keyFn(item);
      if (!map.has(key)) map.set(key, []);
      map.get(key).push(item);
    }
    return map;
  }

  // --- Pass Rate chart ---
  const passData = await getJson('/AI/OutcomesPassRate');
  const years = [...new Set(passData.map(x => x.academicYear))].sort();

  const byCourse = groupBy(passData, x => x.course);
  const courseNames = [...byCourse.keys()].sort();

  const passDatasets = courseNames.map(course => {
    const rows = byCourse.get(course);
    const series = years.map(y => {
      const r = rows.find(x => x.academicYear === y);
      return r ? r.passRate : 0;
    });
    return { label: course, data: series };
  });

  const ctxPass = document.getElementById('chartPassRate');
  new Chart(ctxPass, {
    type: 'bar',
    data: {
      labels: years,
      datasets: passDatasets
    },
    options: {
      responsive: true,
      plugins: {
        legend: { position: 'bottom' }
      },
      scales: {
        y: { beginAtZero: true, max: 100, title: { display: true, text: 'Pass rate (%)' } }
      }
    }
  });

  // --- Avg Grade chart ---
  const avgData = await getJson('/AI/OutcomesAverageGrade');
  const avgYears = [...new Set(avgData.map(x => x.academicYear))].sort();
  const avgByCourse = groupBy(avgData, x => x.course);
  const avgCourseNames = [...avgByCourse.keys()].sort();

  const avgDatasets = avgCourseNames.map(course => {
    const rows = avgByCourse.get(course);
    const series = avgYears.map(y => {
      const r = rows.find(x => x.academicYear === y);
      return r ? r.avgPoints : 0;
    });
    return { label: course, data: series, tension: 0.2 };
  });

  const ctxAvg = document.getElementById('chartAvgGrade');
  new Chart(ctxAvg, {
    type: 'line',
    data: { labels: avgYears, datasets: avgDatasets },
    options: {
      responsive: true,
      plugins: { legend: { position: 'bottom' } },
      scales: {
        y: { beginAtZero: true, title: { display: true, text: 'Avg grade points' } }
      }
    }
  });

  // --- Forecast chart ---
  const forecast = await getJson('/AI/DemandForecast');
  const meta = document.getElementById('forecastMeta');
  meta.textContent = `Years used: ${forecast.yearsUsed.join(', ')} | Forecasting: ${forecast.nextYear}`;

  const top = forecast.data.slice(0, 10); // keep it readable
  const ctxForecast = document.getElementById('chartForecast');
  new Chart(ctxForecast, {
    type: 'bar',
    data: {
      labels: top.map(x => x.course),
      datasets: [
        { label: 'Forecast demand', data: top.map(x => x.forecastDemand) },
        { label: 'Recommended capacity', data: top.map(x => x.recommendedCapacity) }
      ]
    },
    options: {
      responsive: true,
      plugins: { legend: { position: 'bottom' } },
      scales: {
        x: { ticks: { maxRotation: 60, minRotation: 45 } },
        y: { beginAtZero: true }
      }
    }
  });

  // --- Signals chart ---
  const signals = await getJson('/AI/SuccessSignals');
  const signalsMeta = document.getElementById('signalsMeta');
  signalsMeta.textContent = `Attendance ↔ grade correlation: ${signals.correlationAttendance}`;

  const ctxSignals = document.getElementById('chartSignals');
  new Chart(ctxSignals, {
    type: 'bar',
    data: {
      labels: signals.signals.map(x => x.signal),
      datasets: [{ label: 'Signal strength', data: signals.signals.map(x => x.score) }]
    },
    options: {
      indexAxis: 'y',
      responsive: true,
      plugins: { legend: { display: false } },
      scales: { x: { beginAtZero: true, max: 100 } }
    }
  });
})();
