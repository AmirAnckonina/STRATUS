import PropTypes from 'prop-types';
import ArrowDownIcon from '@heroicons/react/24/solid/ArrowDownIcon';
import ArrowUpIcon from '@heroicons/react/24/solid/ArrowUpIcon';
import CurrencyDollarIcon from '@heroicons/react/24/solid/CurrencyDollarIcon';
import ArrowUpRightIcon from '@heroicons/react/24/solid/ArrowUpRightIcon';
import ArrowDownLeftIcon from '@heroicons/react/24/solid/ArrowDownLeftIcon';
import { Avatar, Card, CardContent, Stack, SvgIcon, Typography, LinearProgress, Box } from '@mui/material';
import ArrowTopRightOnSquareIcon from '@heroicons/react/24/solid/ArrowTopRightOnSquareIcon';

export const OverviewAverageDiskUsage = (props) => {
  const { difference, positive = false, sx, value } = props;

  return (
    <Card sx={sx}>
      <CardContent>
        <Stack
          width="100%"
          alignItems="flex-start"
          direction="row"
          justifyContent="space-between"
          spacing={3}
        >
          <Stack spacing={1}>
            <Typography
              color="text.secondary"
              variant="overline"
            >
              Average Disk Space Usage 
            </Typography>
            <Typography variant="h4">
              {value}%
            </Typography>
          </Stack>
          <Avatar
            sx={{
              backgroundColor: 'error.main',
              height: 56,
              width: 56
            }}
          >
            <SvgIcon>
              <ArrowDownLeftIcon />
            </SvgIcon>
          </Avatar>
        </Stack>
          <Box sx={{ mt: 3 }}>
        <LinearProgress
            value={value === 'N/A' ? 0 : value}
            variant="determinate"
            sx={{
              '& .MuiLinearProgress-bar': {
                backgroundColor: 'success.main',
              },
              '& .MuiLinearProgress-barColorPrimary': {
                transition: 'none',
                width: `${value}%`,
              },
            }}
          />
        </Box>
      </CardContent>
    </Card>
  );
};

OverviewAverageDiskUsage.prototypes = {
  difference: PropTypes.number,
  positive: PropTypes.bool,
  sx: PropTypes.object,
  value: PropTypes.string.isRequired
};
